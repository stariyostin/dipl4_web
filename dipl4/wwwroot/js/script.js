// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// JavaScript for label effects only
$(window).load(function () {
    $(".col-3 input").val("");

    $(".input-effect input").focusout(function () {
        if ($(this).val() != "") {
            $(this).addClass("has-content");
        } else {
            $(this).removeClass("has-content");
        }
    })
});

//$(document).ready(function () {
//    var productContainerElement = $('#product-container');
//    var productRowTemplate = productContainerElement.find('.product-row').first().clone();

//    $('.add-product').click(function () {
//        var newProductRow = $('.product-row').first().clone();
//        newProductRow.find('.product-select').val('');
//        newProductRow.find('.product-quantity').val('1');
//        $('#product-container').append(newProductRow);
//    });

//    $(document).on('click', '.remove-product', function () {
//        $(this).closest('.product-row').remove();
//    });
//});