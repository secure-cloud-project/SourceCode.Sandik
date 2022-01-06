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
                    //debugger;
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
                    //debugger;
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

            if (file.size > 40 * 1024 * 1024 ) {
                alert('max upload size is 40MB');
            }

            //var data = new FormData($('form')[0]);
            //sendFile(file);
        });

        const uploadForm = document.getElementById("uploadForm");
        const inpFile = document.getElementById("inpFile");
        var progressBarFill = $("#progressBar .progress-bar-fill")[0];            
        const progressBarText = progressBarFill.querySelector(".progress-bar-text");

        uploadForm.addEventListener("submit", uploadFile);

        function uploadFile(e) {
            e.preventDefault();
            debugger;
            //var frm = $("#uploadForm")[0];
            sendFile();

            //debugger;
            //const xhr = new XMLHttpRequest();


        }

        var sendFile = function () {
            //debugger;
            //document.getElementById('uploadForm').onsubmit = function () {
                var formdata = new FormData(); //FormData object
                var fileInput = document.getElementById('inpFile');
                //Iterating through each files selected in fileInput
                for (i = 0; i < fileInput.files.length; i++) {
                    //Appending each file to FormData object
                    formdata.append(fileInput.files[i].name, fileInput.files[i]);
                }
                //Creating an XMLHttpRequest and sending
                var xhr = new XMLHttpRequest();
            xhr.open('POST', 'UserStorage/UploadFile');

                // burası return ün üzerindeydi
                xhr.upload.addEventListener('progress', function (e) {
                    var percent = e.lengthComputable ? (e.loaded / e.total) * 100 : 0;
                    progressBarFill.style.width = percent.toFixed(2) + "%";
                    progressBarText.textContent = percent.toFixed(2) + "%";
                });

                xhr.send(formdata);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState == 4 && xhr.status == 200) {
                        alert(xhr.responseText);
                    }
                }
               
                return false;
            //}
        }

        //var sendFile_ = function (file) {
        //    var data = new FormData();
        //    var ifile = $("#inpFile")[0].files[0];
        //    data.append(ifile.name, ifile);
        //    //debugger;
        //    $.ajax({
        //        // Your server script to process the upload
        //        url: 'UploadFile',
        //        type: 'POST',

        //        // Form data
        //        data: data, //new FormData($('form')[0]),

        //        // Tell jQuery not to process data or worry about content-type
        //        // You *must* include these options!
        //        cache: false,
        //        contentType: 'multipart/form-data',
        //        processData: false,

        //        // Custom XMLHttpRequest
        //        xhr: function () {
        //            //debugger;
        //            var myXhr = $.ajaxSettings.xhr();
        //            if (myXhr.upload) {
        //                // For handling the progress of the upload
        //                myXhr.upload.addEventListener('progress', function (e) {
        //                    if (e.lengthComputable) {
        //                        $('progress').attr({
        //                            value: e.loaded,
        //                            max: e.total,
        //                        });
        //                    }
        //                }, false);
        //            }
        //            return myXhr;
        //        }
        //    });
        //}   
    }
};

$(function () {
    site.init();
});

