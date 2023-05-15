

$("#login").click(function () {
    var registrationData = {
        email: $("#email").val().toLowerCase().replace(/\s/g, ''),
        password: $("#password").val().toLowerCase(),
     
    };
    console.log($("#email").val().toLowerCase());
    $.ajax({
        url: "/api/auth/signin",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(registrationData),
        success: function (idToken) {
            // Login successful, retrieve the ID token
            console.log("ID token:", idToken);
            $(".invalidPassword").hide();
            $(".invalidEmail").hide();
            localStorage.setItem("idToken", idToken);
            window.location.href = "/dashboard";
            // Use the ID token for further authentication or redirection
            // Perform further actions or redirect to another page
        },
        error: function (xhr, status, error) {
            // Login failed, handle the error
            if (xhr.responseText === "Incorrect password.") {
                // Incorrect password error
                $(".invalidPassword").show();  
                $(".invalidEmail").hide();
            } else if (xhr.responseText === "Email not found.") {
                // Email not found error
                $(".invalidPassword").hide();
                $(".invalidEmail").show();
            } else {
                // Both email and password are incorrect
                $(".invalidPassword").show();
                $(".invalidEmail").show();
            }

            console.error("Login error:", xhr.responseText);
        },
    });

});



 