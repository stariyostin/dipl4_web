$(document).ready(function () {
    // Обработчик клика на кнопку "Добавить товар"
    $('.add-product').click(function () {
        // Клонируем первую строку товара
        var newProductRow = $('.product-row').first().clone();
        // Очищаем значения полей в клонированной строке
        newProductRow.find('select, input').val('');
        // Добавляем новую строку товара в контейнер
        $('#product-container').append(newProductRow);
    });

    // Обработчик клика на кнопку "Удалить товар"
    $(document).on('click', '.remove-product', function () {
        // Удаляем родительскую строку товара
        $(this).closest('.product-row').remove();
    });
});
