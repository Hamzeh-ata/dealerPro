// Initialize Firebase
  $(document).ready(function() {
      function showProducts(category) {
          var url = "products.html?category=" + category;
          $(location).attr('href', url);
      }
});