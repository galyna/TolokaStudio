
var Buyer = {

    buy: function (obj) {
        var order = {
            ProductId: obj.title
        };

        $.ajax({
            type: "post",
            url: "/Order/Create",
            data: order,

            dataType: "json",
            success: function (data) {
                if (data != null) {
                    window.location.reload(data.Url);
                   
                  
                } else {
                    $("#dialog-form").dialog("open");
                }

            },
            failure: function () {
                window.location.reload(true);
            }
        });
    },
    init: function () {
    
////        if ($(ids).length>0) {
////        ids.each(function (i) {
////            $(".ordered" + i).show();
////            $(".order" + i).hide();
////        });
//        }
//         for (var i = 0; i < data.id.length; i++) {
//                            $(".ordered" + data.id[i]).show();
//                            $(".order" + data.id[i]).hide();
//                            
//                        } 

        $(".orderBtn").click(function () {
                Buyer.buy(this);
           });
    }
};
