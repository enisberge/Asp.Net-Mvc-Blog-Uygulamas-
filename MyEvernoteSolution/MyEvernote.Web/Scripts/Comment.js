﻿var noteid = -1;
var modalCommentBodyId = " #modal_comment_body";

$(function () {
    $('#modal_comment').on('show.bs.modal',
        function (e) {
            var btn = $(e.relatedTarget);//tıklanan elementi bize sunar
            noteid = btn.data("note-id");//yakalanan butonun data attributeunu aldık
            $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
        });
});

function doComment(btn, click, commentid, divid) {

    var button = $(btn);
    var mode = button.data("edit-mode");

    if (click === "edit_clicked") {

        if (!mode) {
            button.data("edit-mode", true);
            button.removeClass("btn-warning");
            button.addClass("btn-success");
            var btnSpan = button.find("i");
            btnSpan.removeClass("far fa-edit");
            btnSpan.addClass("fas fa-check");

            $(divid).addClass("editable");
            $(divid).attr("contenteditable", true);
        } else {
            button.data("edit-mode", false);
            button.addClass("btn-warning");
            button.removeClass("btn-success");
            var btnSpan = button.find("i");
            btnSpan.addClass("far fa-edit");
            btnSpan.removeClass("fas fa-check");
            $(divid).removeClass("editable");
            $(divid).attr("contenteditable", false);
            $(divid).focus();

            var txt = $(divid).text();

            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt }
            }).done(function (data) {

                if (data.result) {
                    //yorumlar partial tekrar yüklenir
                    $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
                } else {
                    alert("Yorum Güncellenemedi !");
                }
            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı !");
            });
        }


    } else if (click === "delete_clicked") {
        var dialog_res = confirm("Yorum silinsin mi ?");
        if (!dialog_res) return false;
        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentid
        }).done(function (data) {
            if (data.result) {
                //yorumlar tekrar yükleniyor
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            } else {
                alert("Yorum Silinemedi !");
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı !");
        });


    } else if (click === "new_clicked") {
        var txt = $("#new_comment_text").val();
        $.ajax({
            method: "POST",
            url: "/Comment/Create",
            data: { "text": txt, "noteid": noteid }
        }).done(function (data) {
            if (data.result) {
                //yorumlar tekrar yükleniyor
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            } else {
                alert("Yorum Eklenemedi !");
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı !");
        });


    }

}