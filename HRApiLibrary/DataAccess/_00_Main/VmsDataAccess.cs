using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._00_Main;

public class VmsDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public VmsDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async void _01(string db, string conn)
    {
        await _01SchemaMaker(db, conn); 

    }
    
    //*********************************************************************************
    //--- Private Functions -----------------------------------------------------------
    //*********************************************************************************
    private async Task _01SchemaMaker(string db, string conn)
    {
        var sql = $"CREATE DATABASE IF NOT EXISTS {db}";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Visitors(string db, string conn)
    {
        var sql = $@"CREATE TABLE if not exists {db}.Visitors (
                        Id                  INT PRIMARY KEY AUTO_INCREMENT,
                        LastNm              VARCHAR(60),
                        FirstNm             VARCHAR(60),
                        MidNm               VARCHAR(60),
                        ContactInfo         VARCHAR(150),
                        PicLoc              Varchar(80),
                        VisitPurpose        VARCHAR(100),
                        EmpmasId            int,
                        PersonToVisit       Varchar(80),
                        CheckInTime         DATETIME,
                        CheckOutTime        DATETIME); "; 
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    
    private async Task _01Tables(string db, string conn)
    {
        var sql = $@"CREATE TABLE if not exists {db}.Hosts (
                    Id INT PRIMARY KEY AUTO_INCREMENT,
                    Name VARCHAR(100),
                    Department VARCHAR(100),
                    ContactInfo VARCHAR(150)
                ); "; 
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Visits(string db, string conn)
    {
        var sql = $@"CREATE TABLE if not exists {db}.Visits (
                    VisitID INT PRIMARY KEY AUTO_INCREMENT,
                    VisitorID INT,
                    HostID INT,
                    VisitDate DATE,
                    VisitTime TIME,
                    VisitStatus VARCHAR(50),
                    FOREIGN KEY (VisitorID) REFERENCES Visitors(VisitorID),
                    FOREIGN KEY (HostID) REFERENCES Hosts(HostID)
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Appointments(string db, string conn)
    {
        var sql = $@"

                CREATE TABLE if not exists {db}.Appointments (
                    AppointmentID INT PRIMARY KEY AUTO_INCREMENT,
                    VisitorID INT,
                    HostID INT,
                    AppointmentDate DATE,
                    AppointmentTime TIME,
                    Status VARCHAR(50),
                    FOREIGN KEY (VisitorID) REFERENCES Visitors(VisitorID),
                    FOREIGN KEY (HostID) REFERENCES Hosts(HostID)
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Departments(string db, string conn)
    {
        var sql = $@"

                CREATE TABLE if not exists {db}.Departments (
                    DepartmentID INT PRIMARY KEY AUTO_INCREMENT,
                    DepartmentName VARCHAR(100)
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01SecurityLogs(string db, string conn)
    {
        var sql = $@"

                CREATE TABLE if not exists {db}.SecurityLogs (
                    LogID INT PRIMARY KEY AUTO_INCREMENT,
                    EventType VARCHAR(100),
                    Description TEXT,
                    EventTime DATETIME
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Badges(string db, string conn)
    {
        var sql = $@"

                CREATE TABLE if not exists {db}.Badges (
                    BadgeID INT PRIMARY KEY AUTO_INCREMENT,
                    VisitorID INT,
                    IssueDate DATE,
                    ExpiryDate DATE,
                    BadgeStatus VARCHAR(50),
                    FOREIGN KEY (VisitorID) REFERENCES Visitors(VisitorID)
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Notification(string db, string conn)
    {
        var sql = $@"

                CREATE TABLE if not exists {db}.Notifications (
                    NotificationID INT PRIMARY KEY AUTO_INCREMENT,
                    RecipientID INT,
                    Message TEXT,
                    SentTime DATETIME,
                    Status VARCHAR(50)
                );";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Settings(string db, string conn)
    {
        var sql = $@"
                CREATE TABLE if not exists {db}.Settings (
                    SettingID INT PRIMARY KEY AUTO_INCREMENT,
                    SettingName VARCHAR(100),
                    SettingValue VARCHAR(255)
                );";

        await _sql.ExecuteCmd(sql, new { }, conn);
        
    }
    
}