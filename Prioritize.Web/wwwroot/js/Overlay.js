var OverLay = (function () {
    $.LoadingOverlaySetup({
        
        image: "",
        fontawesome: "fa fas fa-atom fa-spin"
        
    });

    var Show = function () {
        $.LoadingOverlay("show")
    }

    var Hide = function (all) {
        $.LoadingOverlay('hide', all)
    }


    return {
        Show: Show,
        Hide: Hide
    }


}())