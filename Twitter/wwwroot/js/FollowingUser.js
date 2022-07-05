$(document).ready(function () {
    $(document).on("click", "a.follow-user-btn", function () {
        var a = $(this);
        var arguments;
        var userId = a.data("userid");
        var followerCountElement = a.parents("div.content").find(".follower-wrapper .followers .follow-count");
        var followerCount;

        if (a.hasClass("anchor-follow")) {
            $.ajax({
                type: "POST",
                url: "/User/Follow",
                // data to be sent to Action
                data: { "userId": userId },
                // type of data to be returned
                //dataType: "html",
                success: function () {
                    a.html("Following");
                    a.removeClass("anchor-follow");
                    a.addClass("anchor-following");
                    followerCount = parseInt(followerCountElement.attr("data-follower-count"), 10) + 1;
                    followerCountElement.attr("data-follower-count", followerCount);
                    followerCountElement.html(followerCount);
                },
                error: function (req, status, error) {
                    alert(req.responseText);
                }
            });
        }

        else {
            $.ajax({
                type: "POST",
                url: "/User/Unfollow",
                // data to be sent to Action
                data: { "userId": userId },
                // type of data to be returned
                //dataType: "html",
                success: function () {
                    a.html("Follow");
                    a.removeClass("anchor-following");
                    a.addClass("anchor-follow");
                    followerCount = parseInt(followerCountElement.attr("data-follower-count"), 10) - 1;
                    followerCountElement.attr("data-follower-count", followerCount);
                    followerCountElement.html(followerCount);
                },
                error: function (req, status, error) {
                    alert(req.responseText);
                }
            });
        }
    });

    // follow/unfollow for .anchor-following
    $(document).on("mouseover", "a.anchor-following", function () {
        var anchor = $(this);
        anchor.html("Unfollow");
    });

    $(document).on("mouseout", "a.anchor-following", function () {
        var anchor = $(this);
        anchor.html("Following");
    });
});