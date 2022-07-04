$(document).ready(function () {
    $(document).on("click", "a.heart", function () {
        var a = $(this);
        var heartcount;
        var postId;
        var replyId;
        var actionUrl;
        var arguments;

        // for replies
        if (a.hasClass("reply-heart")) {
            replyId = a.data("replyid"); // must use lowercase for data() arguments
            actionUrl = "Reply"; // for now
            arguments = { "replyId": replyId };
        }

        // for posts
        else {
            postId = a.data("postid"); // must use lowercase for data() arguments
            actionUrl = "Post"; // for now
            arguments = { "postId": postId };
        }

        if (a.hasClass("hearted")) {
            actionUrl = actionUrl + "Unheart";

            $.ajax({
                type: "POST",
                url: "/Post/" + actionUrl,
                // data to be sent to Action
                data: arguments,
                // type of data to be returned
                //dataType: "html",
                success: function () {
                    heartcount = parseInt(a.attr("data-heartcount"), 10) - 1;
                    a.attr("data-heartcount", heartcount);
                    a.find(".heart-count").html(heartcount);
                    a.find(".fa-heart").removeClass("fa-solid");
                    a.find(".fa-heart").addClass("fa-regular");
                    a.removeClass("hearted");
                },
                error: function (req, status, error) {
                    alert(actionUrl);
                }
            });
        }

        else {
            actionUrl = actionUrl + "Heart";

            $.ajax({
                type: "POST",
                url: "/Post/" + actionUrl,
                // data to be sent to Action
                data: arguments,
                // type of data to be returned
                //dataType: "html",
                success: function () {
                    heartcount = parseInt(a.attr("data-heartcount"), 10) + 1;
                    a.attr("data-heartcount", heartcount);
                    a.find(".heart-count").html(heartcount);
                    a.find(".fa-heart").removeClass("fa-regular");
                    a.find(".fa-heart").addClass("fa-solid");
                    a.addClass("hearted");
                },
                error: function (req, status, error) {
                    alert(actionUrl);
                }
            });
        }
    });
});