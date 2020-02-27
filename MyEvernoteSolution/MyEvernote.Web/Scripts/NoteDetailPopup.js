
$(function () {
    $('#modal_notedetail').on('show.bs.modal',
        function (e) {
            var btn = $(e.relatedTarget);//tıklanan elementi bize sunar
            noteid = btn.data("note-id");//yakalanan butonun data attributeunu aldık
            $("#modal_notedetail_body").load("/Note/GetNoteText/" + noteid);
        });
});