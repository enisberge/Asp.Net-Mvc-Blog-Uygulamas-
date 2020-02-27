$(function () {

    var noteids = [];

    $("div[data-note-id]").each(function (i, e) {
        //i indexi e elementi temsil ediyor
        noteids.push($(e).data("note-id")); //elementin içindeki note-id değerini al
    });
    $.ajax({
        method: "POST",
        url: "/Note/GetLiked",
        data: { ids: noteids }
    }).done(function (data) {

        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {

                var id = data.result[i];
                var likedNote = $("div[data-note-id=" + id + "]");
                var btn = likedNote.find("button[data-liked]");
                var span = btn.find("i.like-star");

                btn.data("liked", true);
                span.removeClass("far fa-thumbs-up");
                span.addClass("fas fa-heart");
            }
        }
    }).fail(function () {

    });


    $("button[data-liked]").click(function () {
        var btn = $(this);
        var liked = btn.data("liked");
        var noteid = btn.data("note-id");
        var spanStar = btn.find("i.like-star");
        var spanCount = btn.find("span.like-count");

        console.log(liked);
        console.log("like count : " + spanCount.text());


        //controllera ajax işlemini gönderme
        $.ajax({
            method: "POST",
            url: "/Note/SetLikeState", //controllera gidecek
            data:
            {
                "noteid": noteid,
                "liked": !liked
            } //data olarak bunu gönderiyoruz.liked değerinin tersi gidecek. yani like ise unlike,unlike ise like
        }).done(function (data) {
            if (data.hasError) {
                alert(data.errorMessage);
            } else {
                liked = !liked;
                btn.data("liked", liked);
                spanCount.text(data.result);
                console.log("like count(after) : " + spanCount.text());
                spanStar.removeClass("far fa-thumbs-up");
                spanStar.removeClass("fas fa-heart");

                if (liked) {
                    spanStar.addClass("fas fa-heart");
                } else {
                    spanStar.addClass("far fa-thumbs-up");
                }
            }


        }).fail(function () {
            //kullanıcı giriş yapmadan likelamaya kalkabilir
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    });
});