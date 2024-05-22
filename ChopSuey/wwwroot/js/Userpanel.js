$(function () {
   

    $(".editbtn").click(function () {
        $(".edit").prop("readonly", false);
        $(".editbtn").hide();
        $(".uodatebtn").fadeIn(400);
    });





    $(".uodatebtn").click(function () {
    
        let userid = $(this).data("userid");
        let username = $("#username").val();
        let firstname = $("#firstname").val();
        let lastname = $("#lastname").val();
        let phone = $("#phone").val();
        let city = $("#city").val();
        let age = $("#age").val();
        $.post("/Account/UpdateUserProfile", { id: userid, firstname: firstname, lastname: lastname, PhoneNumber: phone, city: city, age: age, username: username }, function (value) {
            $("#popup p").text("تغییر اطلاعات کاربری موفق بود.");

            $("#popup").fadeIn(500);

            $("#popup").delay(3000).fadeOut(500);

            $(".edit").prop("readonly", true);
            $(".uodatebtn").hide();
            $(".editbtn").show();
        });
    });
    $("#change").click(function (event) {

        $(".changepass").fadeIn(500);
    });

    $("#btnclose1").click(function (event) {

        $(".changepass").fadeOut(500);
    });



    $("#changePasswordForm").submit(function (event) {
        event.preventDefault();

        let currentPassword = $("#currentPassword").val();
        let newPassword = $("#newPassword").val();
        let confirmPassword = $("#confirmPassword").val();


        if (newPassword !== confirmPassword) {
            alert("عدم تطبیق رمز عبور جدید با تایید رمز عبور وارد شده");
            $("#confirmPassword").val(null);
            return;
        }


        $.post("/Account/ChangePassword", { currentPassword: currentPassword, newPassword: newPassword }, function (value) {
            alert(value);
            $("#currentPassword").val(null);
            $("#newPassword").val(null);
            $("#confirmPassword").val(null);
        });
    });

    $("#btnclose5").click(function (event) {

        $("#divselect3").fadeOut(500);
    });





    $("#exportPDF").click(function () {
     
        let userId = $("#inpuid").val();
        $.post("/Home/GeneratePdf", { userId: userId }, function (value) {

            console.log(value);
        })
    });



});












