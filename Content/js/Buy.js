
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
               // window.location.reload(data);
                if (data != null) {
                   $(".ordered"+obj.title).show();
                  $(".order" + obj.title).hide();
                }
            },
            failure: function () {
                window.location.reload(true);
            }
        });
    },
    init: function () {
       
        
        $(".orderBtn")
            .click(function () {
                Buyer.buy(this);
            });
    }
};
