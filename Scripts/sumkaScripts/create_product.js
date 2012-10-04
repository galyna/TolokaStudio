
var ProductCreator = {

    init: function () {
        $("#Name").change(function () {
            $(" .box_main_item_text h3").html($(this).val());
        });
        var description = $("#Price").change(function () {
            $(".box_main_item_text span").html($(this).val());
        }); ;

        //set src
        $('#templatesrc').load(function () {
            $(" .box_main_item_img img").attr("src", $(this.contentDocument).find('#ImageUploaded').attr("src"));
            $("#ImagePath").val($(this.contentDocument).find('#ImageUploaded').attr("src"));
           
        });
    }
};
