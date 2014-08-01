function LoadConsole() {
    var curReportFun;
    var mainConsole = $('<div class="console">');
    var lastMessage;
    var curPrompt;
    var controller = mainConsole.console({
        continuedPrompt: true,
        promptLabel: function() {
            return curPrompt ? curPrompt : "> "
        },
        commandValidate: function() {
            return true;
        },
        commandHandle: function (line, reportFn) {
            if (line.trim().toUpperCase() === "CLS") {
                controller.reset();
            }
            else {
                lastMessage = null;
                curReportFun = reportFn;
                _sendCommand(line);
            }

        },
        cancelHandle: function () {
            //sending CTRL+C character (^C) to the server to cancel the current command
            _sendCommand("\x03");
        },
        completeHandle: function (line, reverse) {
            //here comes a list of available completion cmdlets
        },
        userInputHandle: function (keycode) {
            //reset the string we match on if the user type anything other than tab == 9
            if (keycode !== 9 && keycode != 16) {
            }
        },
        cols: 3,
        autofocus: true,
        animateScroll: true,
        promptHistory: true,
        welcomeMessage: "Orchard PowerShell console\r\nType 'cls' to clear the console\r\n\r\n"
    });
    window.$Console = $('#Console');
    window.$Console.append(mainConsole);
    $("div.jquery-console-inner").css("background-color", "#012456");

    var connection = $.connection('/conn/commandstream');
    window.$Console.data('connection', connection);
    controller.disableInput();

    connection.logging = true;
    connection.start({
        waitForPageLoad: true, 
        transport: ['serverSentEvents', 'foreverFrame', 'longPolling' ] // websockets don't work very well here
    }, function() {
        controller.enableInput();
    });


    connection.received(function (data) {
        DisplayAndUpdate(data);
    });

    function _sendCommand(input) {
        connection.send(input);
    }

    function getJSONValue(input) {
        return input ? (input.Output ? input.Output : "") : "";
    }
    function endsWith(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }

    function startsWith(str, prefix) {
        return str.indexOf(prefix) == 0;
    }
    
    function DisplayAndUpdate(data) {
        var line = getJSONValue(data);
        var hasNewLine = endsWith(line, '\n');

        curPrompt = data.Path ? data.Path + "> " : "> ";
        line = line.replace(/[\r\n]+$/, '').replace(/(\r\n|\n|\r)/gm, "<br/>");

        if (line.length > 0) {
            if (!lastMessage && curReportFun)
                curReportFun("", "jquery-console-message-value");

            lastMessage = lastMessage ? lastMessage : $(".jquery-console-message").last();

            var part = $("<span></span>").html(line).css("display", "inline-block")

            if (data.BackColor) {
                part.css("background-color", data.BackColor);
            }

            if (data.ForeColor) {
                part.css("color", data.ForeColor);
            } else {
                switch (data.Type) {
                case 2 /* ERROR */:
                    part.css("color", "red");
                    break;
                case 1 /* WARNING*/:
                    part.css("color", "yellow");
                    break;
                case 4 /* VERBOSE*/:
                    part.css("color", "gray");
                    break;
                default:
                    part.css("color", "white");
                }
            }

            lastMessage.append(part);
        }

        if (hasNewLine && lastMessage) {
            lastMessage.append("</br>");
        } else {
            if (!lastMessage && curReportFun)
                curReportFun("", "");
        }

        $(".jquery-console-prompt-label").last().text(curPrompt);
        controller.scrollToBottom();
    }
}

$(function () {
    LoadConsole();
})
