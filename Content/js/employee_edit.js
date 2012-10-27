
var Employee = {

    addProductInit: function () {

        var employee = {
            employeeId: $(".addProduct")[0].alt
        };

        $(".addProduct").unbind("click");
        $(".addProduct").click(function () {
            $.ajax({
                type: "POST",
                url: "/Product/Create",
                data: employee,

                dataType: "json",
                success: function (data) {
                    window.location.reload(data);
                },
                failure: function () {
                    window.location.reload(true);
                }
            });

        });
    },

    editInit: function () {
        var employee = {
           id: $(".editDetailsBtn")[0].alt
        };
       $(".editDetailsBtn").unbind("click");
       $(".editDetailsBtn").click(function () {
            $.ajax({
                type: "Get",
                url: "/Employee/EditDetails/",
                data: employee,

                dataType: "json",
                success: function (data) {
                    window.location.reload(data);
                },
                failure: function () {
                    window.location.reload(true);
                }
            });
        });
    },
    init: function () {
        $("#FirstName").change(function () {
            var first = $("#FirstName").val();
            var last = $("#LastName").val();
            $(".box_main_item_text .firstname").html(first);

            $(".deleteBtn").attr("title", "Видалити автора " + first + " " + last);
            $(".authorUploadBtn").attr("title", "Завантажити аватарку " + first + " " + last);
            $(".addProduct").attr("title", "Додати продукт " + first + " " + last);
            $(".editDetailsBtn").attr("title", "Редагувати сторінку " + first + " " + last);

        });
        $("#LastName").change(function () {
            $(".box_main_item_text .lastName").html($("#LastName").val());
        });
        $("#Email").change(function () {
            $(".box_main_item_text .email").html($("#Email").val());
        });


        $(".box_main_item_img img").attr("src", $(this.contentDocument).find('#ImageUploaded').attr("src"));
        $("#ImagePath").val($(this.contentDocument).find('#ImageUploaded').attr("src"));

             
      Employee.addProductInit();

      Employee.editInit();

    }
};