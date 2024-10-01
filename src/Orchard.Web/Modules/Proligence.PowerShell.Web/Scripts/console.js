function LoadConsole() {
    var tabIndex = 0;
    var tabCompletionBase = "";
    var tabCompletionData;
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
            _sendAbortCommand();
        },
        completeHandle: function (line, reverse) {
            if (tabCompletionBase.length == 0) {
                tabCompletionBase = line;
                connection.send({ Command: tabCompletionBase, Position: controller.cursorPosition(), Completion: true });
            } else {
                CompleteCommand(tabCompletionData)
            }
            return "";
        },
        cols: 3,
        autofocus: true,
        animateScroll: true,
        promptHistory: true,
        welcomeMessage: "Orchard PowerShell console\r\nType 'cls' to clear the console\r\n\r\n"
    });

    var progressBar = $("#Progress");
    var progressLabel = $(".progress-label");

    var initialProgressText = progressLabel.text();

    progressBar.progressbar({
        value: false
    });


    // hooking into keypress events on the underlying textarea
    controller.typer.keydown(function (e) {
        var keyCode = e.keyCode;
        if (e.keyCode != 9) {
            tabIndex = 0;
            tabCompletionBase = "";
            tabCompletionData = null;
        }
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
        if (data.Type == 6) {
            tabCompletionData = data.Data;
            CompleteCommand(data.Data)
        } else if (data.Type == 5) {
            progressBar.progressbar("value", data.Data.percent);
            progressLabel.text(data.Data.activity + ": " + data.Data.status)
        } else {
            DisplayAndUpdate(data);
        }
    });

    function _sendCommand(input) {
        progressBar.show()
        progressBar.progressbar("value", 100)
        progressLabel.text(initialProgressText + " " + input)
        connection.send({ Command: input });
    }

    function _sendAbortCommand() {
        _sendCommand("\x03")
        progressBar.show()
        progressBar.progressbar("value", 100)
        progressLabel.text("Aborting current command...");
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

    function CompleteCommand(data) {
        if (!data.Results || data.Results.length == 0)
            return;

        if (tabIndex >= data.Results.length)
            tabIndex = 0;

        var prefix = tabCompletionBase.substring(0, data.ReplacementIndex);
        var infix = data.Results[tabIndex];
        var suffix = data.ReplacementLength > 0 ? tabCompletionBase.substring(data.ReplacementIndex + data.ReplacementLength) : "";
        controller.promptText(prefix + infix + suffix);

        controller.moveCursor(suffix.length * -1);
        controller.updatePromptDisplay();

        tabIndex++;
    }
    
    function DisplayAndUpdate(data) {
        var line = getJSONValue(data);
        var hasNewLine = data.NewLine ? true : false;

        // building prompt
        curPrompt = data.Prompt ? data.Prompt : "> ";
        line = line
            .replace(/[\r\n]+$/, '')
            .replace(/(\r\n|\n|\r)/gm, "<br/>")
            .replace(/\s/g, "&nbsp;");

        if (!data.Finished) {
            if (!lastMessage && curReportFun)
                curReportFun("", "jquery-console-message-value");

            lastMessage = lastMessage ? lastMessage : $(".jquery-console-message").last();

            var part = $("<span></span>").html(line).css("display", "inline-block");

            if (data.BackColor && data.BackColor.toUpperCase() != "BLUE") {
                part.css("background-color", data.BackColor);
            }

            if (data.ForeColor) {
                part.css("color", data.ForeColor);
            } else {
                switch (data.Type) {
                 case 1 /* WARNING*/:
                        part.addClass("jquery-console-message-warning")
                        break;
                    case 2 /* ERROR */:
                        part.addClass("jquery-console-message-error")
                        break;
                    case 3 /* DEBUG */:
                        part.addClass("jquery-console-message-debug")
                        break;
                    case 4 /* VERBOSE*/:
                        part.addClass("jquery-console-message-verbose")
                        break;
                    default:
                        part.addClass("jquery-console-message-value")
                    }
            }

            lastMessage.append(part);

            if (hasNewLine) {
                lastMessage.append("</br>");
            }
        } else {
            if (!lastMessage) {
                curReportFun('', '');
            }

            progressLabel.text(initialProgressText);
            progressBar.hide()
        }

        $(".jquery-console-prompt-label").last().text(curPrompt);
        controller.scrollToBottom();
    }
}

$(function () {
    LoadConsole();
})
