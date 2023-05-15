$('#searchForProducts').keypress(function (e) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        // Enter key was pressed
        // Do something with inputValue
        e.preventDefault(); // Prevent form submission
        var searchQuery = $(this).val(); // Get search query
        window.location.href = 'products?search=' + encodeURIComponent(searchQuery); // Redirect to product page with search parameter in URL

    }
});

$(".categorieCard").click(function () {
    //change mainImg based on clicked card id
    var cardId = $(this).attr("id");
    window.location.href = encodeURIComponent(cardId); // Redirect to product page with search parameter in URL

});



 