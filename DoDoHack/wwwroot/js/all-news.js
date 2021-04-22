$(function () {
    var quill = new Quill('#editor', {
        modules: {
            toolbar: [
                ['bold', 'italic', 'underline', 'strike'],
                ['image', 'blockquote', 'code-block'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                [{ 'indent': '-1' }, { 'indent': '+1' }],
                [{ 'size': ['small', false, 'large', 'huge'] }],
                [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                [{ 'color': [] }, { 'background': [] }],
                [{ 'font': [] }],
                [{ 'align': [] }],
                ['formula'],
                ['clean']
            ]
        },
        placeholder: 'Текст новости',
        theme: 'snow'
    });

    $("#create-news #create-news-slide").on("click", function () {
        $("#create-news form").slideToggle();
    });

    $("#create-news form").on("submit", function (e) {
        $("#news-body").html(quill.getText());
    });
});