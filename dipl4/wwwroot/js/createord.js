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
$(document).ready(function () {
    // Обработчик клика на кнопку "Добавить товар"
    $('.add-product').click(function () {
        // Клонируем первую строку товара
        var newProductRow = $('.product-row').first().clone();
        // Очищаем значения полей в клонированной строке
        newProductRow.find('select, input').val('');
        // Добавляем новую строку товара в контейнер
        $('#product-container').append(newProductRow);
        // Показать кнопку "Удалить" для клонированной строки
        newProductRow.find('.remove-product').prop('disabled', false);
    });

    // Обработчик клика на кнопку "Удалить товар"
    $(document).on('click', '.remove-product', function () {
        // Удаляем родительскую строку товара
        $(this).closest('.product-row').remove();
        // Проверяем количество строк товара после удаления
        var remainingRowsCount = $('.product-row').length;
        // Если остается только одна строка
        if (remainingRowsCount === 1) {
            // Делаем кнопку удаления в первой строке неактивной
            $('.product-row .remove-product').prop('disabled', true);
        }
    });
});
