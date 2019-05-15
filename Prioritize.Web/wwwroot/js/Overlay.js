var OverLay = (function () {
    $.LoadingOverlaySetup({
        fontawesomeAutoResize: true,
        image: "",
        fontawesome: "fa fas fa-atom fa-spin"
        
    });

    var Show = function (ele) {
        $.LoadingOverlay("show")
    }

    var Hide = function (all, ele) {
            $.LoadingOverlay('hide')
    }


    return {
        Show: Show,
        Hide: Hide
    }


}())