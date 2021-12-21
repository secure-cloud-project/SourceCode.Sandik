site = {
    init: function () {
        $("#btnLogin").unbind().click(function (e) {
            e.preventDefault();
            var param = {
                Mail: $("[name=username]").val(),
                Password: $("[name=password]").val()
            }

            $.ajax({
                type: "POST",
                url: "/Secure/Login",
                data: param,
                success: function (v) {
                    if (v.isOk) {
                        location.href = "/Secure/LoginTwoFactor";
                        //alert("Login işlemi başarılı");
                    }
                    else
                        alert("Mail veya şifre hatalı");
                },
                error: function () {
                    alert("Beklenmeyen bir hata meydana geldi.");
                }
            });
        });

        $("#btnSignIn").click(function (e) {
            e.preventDefault();
            if ($("[name=password]").val() != $("[name=passwordTekrar]").val()) {
                alert("Şifreler aynı değil");
                return;
            }
            var param = {
                Name: $("[name=ad]").val(),
                SurName: $("[name=soyad]").val(),
                Mail: $("[name=mail]").val(),
                Password: $("[name=password]").val()
            }            
            $.ajax({
                type: "POST",
                url: "/Secure/SignIn",
                data: param,
                success: function (v) {
                    debugger;
                    if (v.isOk)
                        location.href = "/Secure/Login";
                    else
                        alert(v.Msj);
                },
                error: function () {
                    alert("Beklenmeyen bir hata meydana geldi.");
                }
            });
        });

        $("#btnTwoFactor").click(function (e) {
            e.preventDefault();
            var param = {
                SecureCode: $("#twoFactorCode").val(),
            }
            $.ajax({
                type: "POST",
                url: "/Secure/LoginTwoFactor",
                data: param,
                success: function (v) {
                    debugger;
                    if (v.isOk)
                        location.href = "/UserStorage";
                    else
                        alert(v.Msj);
                },
                error: function () {
                    alert("Beklenmeyen bir hata meydana geldi.");
                }
            });

        });  

        $(':file').on('change', function () {
            var file = this.files[0];

            if (file.size > 1024) {
                alert('max upload size is 1k');
            }

            // Also see .name, .type
        });

        $(':button').on('click', function () {
            $.ajax({
                // Your server script to process the upload
                url: 'UserStorage/UploadFile',
                type: 'POST',

                // Form data
                data: new FormData($('form')[0]),

                // Tell jQuery not to process data or worry about content-type
                // You *must* include these options!
                cache: false,
                contentType: false,
                processData: false,

                // Custom XMLHttpRequest
                xhr: function () {
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) {
                        // For handling the progress of the upload
                        myXhr.upload.addEventListener('progress', function (e) {
                            if (e.lengthComputable) {
                                $('progress').attr({
                                    value: e.loaded,
                                    max: e.total,
                                });
                            }
                        }, false);
                    }
                    return myXhr;
                }
            });
        });

    }
};

$(function () {
    site.init();
});

