$(() => {


    $(".like").on('click', function () {

        $.post("/home/likequestion", { questionId: $(this).data('id') }, function (like) {

            $("#likes").text(like.likes)
        })
        $("#heart").removeClass('glyphicon-heart-empty');
        $("#heart").addClass('glyphicon-heart');
    })

    $(".like-answer").on('click', function () {

        const answerId = $(this).data('id')

        $.post("/home/likeanswer", { answerId }, function (like) {

            $(`#${answerId}`).text(like.likes)
        })
        $(`#heart-${answerId}`).removeClass('glyphicon-heart-empty');
        $(`#heart-${answerId}`).addClass('glyphicon-heart');
    })

})