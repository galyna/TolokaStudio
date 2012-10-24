
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
                    window.location.reload(true);
                }

            },
            failure: function () {
                window.location.reload(true);
            }
        });
    },
    makeaOrder: function (id) {
        var order = {
            OrderId: id,
            Comments: $("textarea[name=" + id + "comments]").val(),
            Email: $("textarea[name=" + id + "email]").val()
        };

        $.ajax({
            type: "post",
            url: "/Order/MakeOrder",
            data: order,
            dataType: "json",
             success: function (data) {
                if (data != null) {
                    window.location.reload(data);


                } 

            }
        });
    },
  
    init: function () {

        $(".orderBtn").click(function () {
            Buyer.buy(this);
        });
    }


};
