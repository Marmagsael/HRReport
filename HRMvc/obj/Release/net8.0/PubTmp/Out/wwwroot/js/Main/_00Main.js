$(document).ready(function () {
    var IsCollapsed     = true;
    var IsLargeScreen   = true;
    var resizeTimer;
    var windowsWidth    = 0;

    windowsWidth        = $(window).width();

    $(window).on('resize', function (e) {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            windowsWidth = $(window).width();
            console.log("windowsWidth: " + windowsWidth);
            _00HoverLeftMenu();  // Re-initialize hover functionality on resize
        }, 250);
    });



    // Initial hover functionality setup
    _00HoverLeftMenu();

    function _00HoverLeftMenu() {
        // First, unbind any previous event listeners to prevent multiple bindings
        $("#mainLeftMenu").off('mouseenter mouseleave');

        if (windowsWidth > 768) {
            $("#mainLeftMenu").mouseenter(function () {
                if (!$(this).hasClass("notHovered")) {
                    $(this).addClass("active");
                    $(this).children().children(".lm-dtls").children('.lmd-content').children('.lmd-subcontent-wrapper').removeClass('d-none');
                }
            }).mouseleave(function () {
                $(this).removeClass("active");
                $(this).children().children(".lm-dtls").children('.lmd-content').children('.lmd-subcontent-wrapper').addClass('d-none');
            });
        } else {
            // Optionally, add functionality for when the window width is 768 or less
        }
    }

    // Hide subcontent on initial load
    $("#mainLeftMenu").children().children(".lm-dtls").children('.lmd-content').children('.lmd-subcontent-wrapper').addClass('d-none');

    function _01ToggleMainMenuInSmallScreen() {
        $("#menu-toggle-icon").click(function () {
            $(this).toggleClass("active");
            $("#mainLeftMenu").toggleClass("active");
            $(".lmd-content").removeClass("active");
            _04MakeMenuItemActiveByUrl();
        });
    };

    function _02ToggleMainMenuInLargeScreen() {
        $("#lm-toggle-menu").click(function () {
            $("#mainLeftMenu").toggleClass("lg-active");
            $("#mainPageContent").toggleClass("mainLeftMenuIsActive");


            $(".lmd-content").removeClass("active");
            _04MakeMenuItemActiveByUrl();

        })
    }




    function _03ToggleSubMenu() {
        $('.lmd-hdr').click(function (event) {
            console.log("click")
            event.stopPropagation();
            let dwn = $(this).parent().children().children().children('.lmd-hdr-icon2').children("i:last");
            let dwnIsVisible = $(dwn).is(":visible");

            $('.lmd-content').removeClass("active");
            $(this).parent().toggleClass("active");

            if (dwnIsVisible) {
                $(this).parent().addClass("active");
                $(this).parent('.lmd-content').children('.lmd-subcontent-wrapper').removeClass('d-none');
            } else {
                $(this).parent().removeClass("active");
                $(this).parent('.lmd-content').children('.lmd-subcontent-wrapper').addClass('d-none');

            }
        });


    }

    function _04MakeMenuItemActiveByUrl() {
        var currentpath = location.pathname;
        console.log(">>", currentpath)
        $('.lm-body .lm-dtls .lmd-content').each(function () {
            if ($(this).children('.lmd-subcontent-wrapper').length > 0) {
                $('.lm-body .lm-dtls .lmd-content .lmd-subcontent-wrapper .lmd-subcontent .lmd-subcontent-item ').each(function () {
                    let submenuPath = $(this);
                    // if the current path is like this link, make it active
                    if (submenuPath.attr('href').indexOf(currentpath) !== -1) {
                        $(this).addClass('active');
                        $(this).parent().parent().parent().addClass("active");
                    }

                })
            }
        })
    };

    //---------------------------------------------------
    //-------- Function for Dropdown Element -----------
    //---------------------------------------------------
    function _05ToggleDropDown() {
        $('.with-dropdown').find('.b-dropdown').click(function () {
            var parent = $(this).parent();
            //remove active class from another element -----------
            $('.with-dropdown').not(parent).removeClass('active');
            // Toggle active class for the clicked element
            $(this).parent('.with-dropdown').toggleClass('active');
        });
    };

    $('.with-dropdown').click(function (event) {
        event.stopPropagation();
    });

    function _06HideAllDropDown() {
        $(window).click(function () {
            $('.with-dropdown').removeClass('active');
        });

    }

    function setFocus(element) {
        element.focus();
    }


    _00HoverLeftMenu();
    _01ToggleMainMenuInSmallScreen();
    _02ToggleMainMenuInLargeScreen();
    _03ToggleSubMenu();
    _04MakeMenuItemActiveByUrl();
    _05ToggleDropDown();
    _06HideAllDropDown();




    

})

