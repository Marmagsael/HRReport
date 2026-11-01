using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using SkiaSharp;

namespace HRMvc.StartupConfig;

// -- Employee photo (14/2052 Upload image) : original + 96x96 icon in Backblaze, the current one recorded in {Code}_Pis.EmpmasPic.
//    Backblaze : {OldPis}/employees/{EmpNumber}/photo/{yyyyMMdd-HHmmss}.{jpg|png} and the same name with _icon.jpg --
public class EmployeePhotoService
{
    public  const long   MaxBytes = 3 * 1024 * 1024;
    public  const int    MinSide  = 100;        // -- smallest width / height (the icon is 96px) --
    private const int    IconSize = 96;
    private const int    MaxSide  = 200;        // -- the stored photo : longest side cut down to this, aspect kept --
    private const int    Keep     = 3;          // -- photos kept per employee; older ones are deleted from Backblaze and EmpmasPic --
    private const string Category = "Profile";

    private readonly BackblazeB2Service             _b2;
    private readonly I_10_EmpmasDataAccess          _empmas;
    private readonly ILogger<EmployeePhotoService>  _log;

    public EmployeePhotoService(BackblazeB2Service b2, I_10_EmpmasDataAccess empmas, ILogger<EmployeePhotoService> log)
    {
        _b2     = b2;
        _empmas = empmas;
        _log    = log;
    }

    // -- Returns null when saved, otherwise the reason --
    public async Task<string?> UploadAsync(UserClaimsModel uc, OEmpmasLite emp, string fileName, byte[] data)
    {
        if (data.Length == 0)          return "The file is empty.";
        if (data.Length > MaxBytes)    return "The file is larger than 3 MB.";
        if (string.IsNullOrWhiteSpace(uc.OpisDb) || string.IsNullOrWhiteSpace(uc.SchemaUserPis)) return "The company has no PIS schema.";

        // -- JPG / PNG only (checked from the content, not the file name), at least MinSide x MinSide --
        var check = Check(data);
        if (check.Error != null) return check.Error;
        string ext = check.Info!.Value.Format == SKEncodedImageFormat.Png ? ".png" : ".jpg";

        var icon = MakeIcon(data);
        if (icon == null) return "The image could not be read.";

        var empmas = await EnsureEmpmasAsync(uc, emp);
        if (empmas?.Id == null) return $"PIS Empmas for {emp.EmpNumber} could not be created.";

        var stamp    = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var folder   = $"{uc.OpisDb!.Trim()}/employees/{emp.EmpNumber!.Trim()}/photo";
        var original = $"{folder}/{stamp}{ext}";

        var stored = Shrink(data, check.Info.Value.Format) ?? data;
        await _b2.UploadAsync(original, stored, ext == ".png" ? "image/png" : "image/jpeg");
        await _b2.UploadAsync(IconName(original), icon, "image/jpeg");

        await _empmas._01EmpmasPic(new EmpmasPicModel
        {
            EmpmasId    = empmas.Id,
            PicName     = Trunc(Path.GetFileName(fileName), 100),
            PicAddress  = original,
            Category    = Category,
            UploadedBy  = uc.UserId,
        }, uc.SchemaUserPis, uc.Conn);

        await PruneAsync(uc, empmas.Id);
        return null;
    }

    // -- Keep the newest Keep photos; a row is removed only after its files are gone, so a failed delete is tried again next upload --
    private async Task PruneAsync(UserClaimsModel uc, int? empmasId)
    {
        var pics = await _empmas._02EmpmasPicList(empmasId, Category, uc.SchemaUserPis, uc.Conn);
        foreach (var old in pics.Skip(Keep))
        {
            if (old == null) continue;
            try
            {
                if (!string.IsNullOrWhiteSpace(old.PicAddress))
                {
                    await _b2.DeleteAsync(old.PicAddress);
                    await _b2.DeleteAsync(IconName(old.PicAddress));
                }
                await _empmas._04EmpmasPic(old.Id, uc.SchemaUserPis, uc.Conn);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Old photo {Address} (EmpmasPic {Id}) was not deleted; it is tried again on the next upload.", old.PicAddress, old.Id);
            }
        }
    }

    // -- Current photo (or its icon) of an employee; null when there is none --
    public async Task<(byte[] Data, string ContentType)?> GetAsync(UserClaimsModel uc, string empNumber, bool icon)
    {
        if (string.IsNullOrWhiteSpace(uc.SchemaUserPis) || string.IsNullOrWhiteSpace(empNumber)) return null;

        var empmas = await _empmas._02EmpmasInternalByEmpNumber(empNumber.Trim(), uc.SchemaUserPis, uc.Conn);
        if (empmas?.Id == null) return null;

        var pic = await _empmas._02EmpmasPicCurrent(empmas.Id, Category, uc.SchemaUserPis, uc.Conn);
        if (string.IsNullOrWhiteSpace(pic?.PicAddress)) return null;

        return await _b2.DownloadAsync(icon ? IconName(pic.PicAddress) : pic.PicAddress);
    }

    // -- Format and pixel size from the file content; Error when it cannot be uploaded (also used by the upload dialog) --
    public static (ImageDetails? Info, string? Error) Check(byte[] data)
    {
        if (data.Length == 0)       return (null, "The file is empty.");
        if (data.Length > MaxBytes) return (null, "The file is larger than 3 MB.");

        using var codec = SKCodec.Create(new SKMemoryStream(data));
        if (codec == null || codec.EncodedFormat is not (SKEncodedImageFormat.Jpeg or SKEncodedImageFormat.Png))
            return (null, "Only JPG or PNG images can be uploaded.");

        var info = new ImageDetails(codec.EncodedFormat, codec.Info.Width, codec.Info.Height, data.Length);
        if (Math.Min(info.Width, info.Height) < MinSide)
            return (info, $"The image is too small ({info.Width} × {info.Height} px). It must be at least {MinSide} × {MinSide} px.");
        return (info, null);
    }

    // -- Whether the employee has a current photo (database only, nothing downloaded) --
    public async Task<bool> HasPhotoAsync(UserClaimsModel uc, string? empNumber)
    {
        if (string.IsNullOrWhiteSpace(uc.SchemaUserPis) || string.IsNullOrWhiteSpace(empNumber)) return false;
        var empmas = await _empmas._02EmpmasInternalByEmpNumber(empNumber.Trim(), uc.SchemaUserPis, uc.Conn);
        if (empmas?.Id == null) return false;
        return !string.IsNullOrWhiteSpace((await _empmas._02EmpmasPicCurrent(empmas.Id, Category, uc.SchemaUserPis, uc.Conn))?.PicAddress);
    }

    public static string IconName(string original) =>
        $"{original[..^Path.GetExtension(original).Length]}_icon.jpg";

    // -- Longest side down to MaxSide, upright, same format (PNG keeps its transparency); null when it is already small enough --
    private static byte[]? Shrink(byte[] data, SKEncodedImageFormat format)
    {
        using var codec = SKCodec.Create(new SKMemoryStream(data));
        if (codec == null || Math.Max(codec.Info.Width, codec.Info.Height) <= MaxSide) return null;
        using var decoded = SKBitmap.Decode(codec);
        if (decoded == null) return null;

        using var upright = Orient(decoded, codec.EncodedOrigin);
        double scale = (double)MaxSide / Math.Max(upright.Width, upright.Height);
        var size = new SKImageInfo(Math.Max(1, (int)Math.Round(upright.Width * scale)), Math.Max(1, (int)Math.Round(upright.Height * scale)));

        using var small = upright.Resize(size, new SKSamplingOptions(SKCubicResampler.Mitchell));
        if (small == null) return null;
        using var image = SKImage.FromBitmap(small);
        return format == SKEncodedImageFormat.Png
             ? image.Encode(SKEncodedImageFormat.Png, 100).ToArray()
             : image.Encode(SKEncodedImageFormat.Jpeg, 90).ToArray();
    }

    // -- Square crop from the centre, upright (phone photos carry their rotation in EXIF), 96x96 JPEG --
    private static byte[]? MakeIcon(byte[] data)
    {
        using var codec = SKCodec.Create(new SKMemoryStream(data));
        if (codec == null) return null;
        using var decoded = SKBitmap.Decode(codec);
        if (decoded == null) return null;

        using var upright = Orient(decoded, codec.EncodedOrigin);
        int side = Math.Min(upright.Width, upright.Height);
        var crop = new SKRectI((upright.Width - side) / 2, (upright.Height - side) / 2, (upright.Width - side) / 2 + side, (upright.Height - side) / 2 + side);

        using var square = new SKBitmap(side, side);
        if (!upright.ExtractSubset(square, crop)) return null;

        using var small = square.Resize(new SKImageInfo(IconSize, IconSize), new SKSamplingOptions(SKCubicResampler.Mitchell));
        if (small == null) return null;

        // -- JPEG has no transparency : a transparent PNG gets a white background instead of black --
        using var flat   = new SKBitmap(IconSize, IconSize);
        using var canvas = new SKCanvas(flat);
        canvas.Clear(SKColors.White);
        canvas.DrawBitmap(small, 0, 0);

        using var image = SKImage.FromBitmap(flat);
        return image.Encode(SKEncodedImageFormat.Jpeg, 85).ToArray();
    }

    private static SKBitmap Orient(SKBitmap src, SKEncodedOrigin origin)
    {
        int degrees = origin switch
        {
            SKEncodedOrigin.BottomRight => 180,
            SKEncodedOrigin.RightTop    => 90,
            SKEncodedOrigin.LeftBottom  => 270,
            _                           => 0,
        };
        if (degrees == 0) return src.Copy();

        bool swap = degrees != 180;
        var dst = new SKBitmap(swap ? src.Height : src.Width, swap ? src.Width : src.Height);
        using var canvas = new SKCanvas(dst);
        canvas.Translate(dst.Width / 2f, dst.Height / 2f);
        canvas.RotateDegrees(degrees);
        canvas.Translate(-src.Width / 2f, -src.Height / 2f);
        canvas.DrawBitmap(src, 0, 0);
        return dst;
    }

    // -- PIS Empmas link by EmpNumber, made from the old PIS name when missing (same as Personal Data / Detailed Address) --
    private async Task<EmpmasInternalModel?> EnsureEmpmasAsync(UserClaimsModel uc, OEmpmasLite emp)
    {
        var empmas = await _empmas._02EmpmasInternalByEmpNumber(emp.EmpNumber, uc.SchemaUserPis, uc.Conn);
        if (empmas != null) return empmas;

        await _empmas._01EmpmasInternal(new EmpmasInternalModel
        {
            EmpNumber   = emp.EmpNumber,
            EmpLastNm   = emp.EmpLastNm,
            EmpFirstNm  = emp.EmpFirstNm,
            EmpMidNm    = emp.EmpMidNm,
            Suffix      = emp.Suffix ?? "",
            EmpAlias    = emp.EmpAlias,
        }, uc.SchemaUserPis, uc.Conn);
        return await _empmas._02EmpmasInternalByEmpNumber(emp.EmpNumber, uc.SchemaUserPis, uc.Conn);
    }

    private static string? Trunc(string? s, int len) => s == null || s.Length <= len ? s : s[..len];
}

public readonly record struct ImageDetails(SKEncodedImageFormat Format, int Width, int Height, long Bytes)
{
    public string SizeText => Bytes >= 1024 * 1024 ? $"{Bytes / 1024d / 1024d:0.00} MB" : $"{Math.Max(1, Bytes / 1024d):0} KB";
}

// -- The employee fields the upload needs (from the old PIS record on 14/2052) --
public record OEmpmasLite(string? EmpNumber, string? EmpLastNm, string? EmpFirstNm, string? EmpMidNm, string? Suffix, string? EmpAlias);
