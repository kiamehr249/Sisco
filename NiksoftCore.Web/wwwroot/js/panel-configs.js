$(document).ready(function () {
    //$('.datepicker').datepicker({
    //    changeMonth: true,
    //    changeYear: true,
    //    dateFormat: 'yy/mm/dd'
    //});

    $('input.emp-check').on('focusout', function () {
        if ($(this).val() != '') {
            $(this).removeClass('is-invalid');
            $(this).addClass('is-valid');
        } else {
            $(this).removeClass('is-valid');
            $(this).addClass('is-invalid');
        }
    });

    $('.datepicker').persianDatepicker({
        format: 'YYYY/MM/DD',
        autoClose: true,
        initialValue: false
    });

    $('.timepicker').persianDatepicker({
        initialValue: false,
        format: 'HH:mm',
        autoClose: true,
        onlyTimePicker: true
    });

    //$('.colorpicker').colorpicker({
    //    format: 'hex'
    //});

});