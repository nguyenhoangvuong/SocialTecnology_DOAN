$(document).ready(function () {

    var preloadbg = document.createElement("img");
    preloadbg.src = "https://s3-us-west-2.amazonaws.com/s.cdpn.io/245657/timeline1.png";

    $("#searchfield").focus(function () {
        if ($(this).val() == "Search contacts...") {
            $(this).val("");
        }
    });
    $("#searchfield").focusout(function () {
        if ($(this).val() == "") {
            $(this).val("Search contacts...");

        }
    });

    $("#sendmessage input").focus(function () {
        if ($(this).val() == "Send message...") {
            $(this).val("");
        }
    });
    $("#sendmessage input").focusout(function () {
        if ($(this).val() == "") {
            $(this).val("Send message...");

        }
    });


    $(".friend").each(function () {
        $(this).click(function () {
            var childOffset = $(this).offset();
            var parentOffset = $(this).parent().parent().offset();
            var childTop = childOffset.top - parentOffset.top;
            var clone = $(this).find('img').eq(0).clone();
            var top = childTop + 12 + "px";

            $(clone).css({ 'top': top }).addClass("floatingImg").appendTo("#chatbox");

            setTimeout(function () { $("#profile p").addClass("animate"); $("#profile").addClass("animate"); }, 100);
            setTimeout(function () {
                $("#chat-messages").addClass("animate");
                $('.cx, .cy').addClass('s1');
                setTimeout(function () { $('.cx, .cy').addClass('s2'); }, 100);
                setTimeout(function () { $('.cx, .cy').addClass('s3'); }, 200);
            }, 150);

            $('.floatingImg').animate({
                'width': "68px",
                'left': '108px',
                'top': '20px'
            }, 200);

            var name = $(this).find("p strong").html();
            var email = $(this).find("p span").html();
            $("#profile p").html(name);
            $("#profile span").html(email);

            $(".message").not(".right").find("img").attr("src", $(clone).attr("src"));
            $('#friendslist').fadeOut();
            $('#chatview').fadeIn();


            $('#close').unbind("click").click(function () {
                $("#chat-messages, #profile, #profile p").removeClass("animate");
                $('.cx, .cy').removeClass("s1 s2 s3");
                $('.floatingImg').animate({
                    'width': "40px",
                    'top': top,
                    'left': '12px'
                }, 200, function () { $('.floatingImg').remove() });

                setTimeout(function () {
                    $('#chatview').fadeOut();
                    $('#friendslist').fadeIn();
                }, 50);
            });

        });
    });
});
$('.ui.dropdown')
    .dropdown({
        allowAdditions: true
    })
<<<<<<< Updated upstream
=======

$.validator.addMethod(
    "checkContentQuestion",
    function (value, element) {
        $.ajax({
            type: "POST",
            url: "/Question/CheckQuestionContent",
            data: "ContentQuestion=" + value,
            dataType: "html",
            success: function (msg) {
                //If username exists, set response to true
                response = (msg.data == 'true') ? true : false;
            }
        });
        return response;
    },
    "Nội dung không được chứa từ ngữ thô tục"
);

$.fn.form.settings.rules.checkContent = function (value, fieldIdentifiers) {
    $.ajax({
        type: "POST",
        url: "/Question/CheckQuestionContent",
        data: "ContentQuestion=" + value,
        dataType: "html",
        success: function (msg) {
            console.log(msg);
            //If username exists, set response to true
            if (msg === "yes") {
                console.log(msg);
                return false;
            } else {
                return true;
            }
        }
    });
};

>>>>>>> Stashed changes
$('.ui.form')
    .form({
        fields: {
            question_title: {
                identifier: 'question_title',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập tiêu đề!'
                    }
                ]
            },
            question_content: {
                identifier: 'question_content',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập nội dung!'
                    },
                    {
                        type: 'minLength[15]',
                        prompt: 'Tối thiểu 15 ký tự!'
                    }
                ]
            },
            tagsQuestion: {
                identifier: 'tagsQuestion',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng nhập thẻ tags!'
                    },
                    {
                        type: 'maxLength[255]',
                        prompt: 'tối thiểu 255 ký tự'
                    }
                ]
            },
            technologyQuestion: {
                identifier: 'technologyQuestion',
                rules: [
                    {
                        type: 'empty',
                        prompt: 'Vui lòng chọn công nghệ của câu hỏi!'
                    }
                ]
            },
        },
        inline: true,
        on: 'blur'
    })
    ;
$('.large.modal')
    .modal('show')
    ;
ClassicEditor
    .create(document.querySelector('#editor'), {
        toolbar: ['heading', '|', 'codeBlock', 'code', '|', 'bold', 'italic', 'Link', 'bulletedList', 'numberedList', '|', 'outdent', 'indent', '|', 'ImageUpload', 'blockQuote', 'insertTable', 'mediaEmbed', 'undo', 'redo']
    })
    .catch(error => {
        console.log(error);
    });
ClassicEditor
    .create(document.querySelector('#editorPost'), {
        toolbar: ['heading', '|', 'codeBlock', 'code', '|', 'bold', 'italic', 'Link', 'bulletedList', 'numberedList', '|', 'outdent', 'indent', '|', 'ImageUpload', 'blockQuote', 'insertTable', 'mediaEmbed', 'undo', 'redo']
    })
    .catch(error => {
        console.log(error);
    });