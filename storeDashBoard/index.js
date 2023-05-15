$(document).ready(function () {
    var userId;
    var token = localStorage.getItem('idToken');
    console.log(token);
    $.ajax({
        url: '/api/DashBoardData/userid',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ token: token }),
        success: function (response) {
            // Token is valid, handle the successful response
             userId = response; // Retrieve the user ID from the response
            // Perform any necessary actions with the user ID
            console.log(userId);
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                localStorage.removeItem('idToken');

                // Redirect the user to the logout or login page
                window.location.href = '/login'; // Replace '/logout' with the appropriate URL
            } else {
                // Other error occurred, handle the error
                console.error(error);
            }
        }
    });
    $('#logOut').click(function () {
        // Clear the token from local storage

        localStorage.removeItem('idToken');
        window.location.href = '/login';
    });



    $("#createStore").click(function () {
        var storeName=$("#storeName").val();
        $.ajax({
            url: "/api/DashBoardData/create",
            type: "POST",
            data: { UID: userId, storeName: storeName },
            success: function (response) {
                // Handle successful response
                console.log(response);
                console.log("created");
            },
            error: function (xhr, status, error) {
                // Handle error
                console.log(xhr.responseText);
            }
        });
    });





});





 