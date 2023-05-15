

$("#signUp").click(function () {
    var registrationData = {
        email: $("#email").val().toLowerCase().replace(/\s/g, ''),
        password: $("#password").val(),
        userName: $("#userName").val()
    };
    console.log($("#email").val().toLowerCase());
    $.ajax({
        url: '/api/RegisterUser',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(registrationData),
        success: function (response) {
            // Registration successful
          
        },
        error: function (xhr, textStatus, errorThrown) {
            // Handle registration error
            console.log(xhr.responseText);
            if (xhr.responseText === "Email already exists") {
                $(".emailExists").show();
                $(".invaildEmail").hide();
                $(".passwordCount").hide();
                $(".HasNumberAndLetter").hide();
            } else if (xhr.responseText === "Invalid email address") {
           
                $(".emailExists").hide();
                $(".passwordCount").hide();
                $(".HasNumberAndLetter").hide();
                $(".invaildEmail").show();
            
            }
            else if (xhr.responseText === "Password must be at least 6 characters long") {
        
                $(".emailExists").hide();
                $(".invaildEmail").hide();
                $(".HasNumberAndLetter").hide();
                $(".passwordCount").show();
            }

            else if (xhr.responseText === "Password contains both numbers and letters") {

                $(".emailExists").hide();
                $(".invaildEmail").hide();
                $(".passwordCount").hide();
                $(".HasNumberAndLetter").show();
            }

            else {
                $(".emailExists").hide();
                $(".invaildEmail").hide();
                $(".passwordCount").hide();
                $(".HasNumberAndLetter").hide();
            }
        }
    });

});



 