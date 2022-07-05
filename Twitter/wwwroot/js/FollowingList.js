$(document).ready(function () {
    $(document).on("click", "a.follow-list-btn", function () {
        var a = $(this);
        var arguments;
        var listId = a.data("listid");
        var followerCountElement = $(".members .follower-count");
        var followerCount;

        if (a.hasClass("anchor-follow")) {
            $.ajax({
                type: "POST",
                url: "/List/Follow",
                // data to be sent to Action
                data: { "listId": listId },
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
                url: "/List/Unfollow",
                // data to be sent to Action
                data: { "listId": listId },
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
    $(document).on("mouseover", "a.follow-list-btn.anchor-following", function () {
        var anchor = $(this);
        anchor.html("Unfollow");
    });

    $(document).on("mouseout", "a.follow-list-btn.anchor-following", function () {
        var anchor = $(this);
        anchor.html("Following");
    });
});