function setMarkdown(element, valueAccessor) {
    var value = valueAccessor();

    var markdown = ko.utils.unwrapObservable(value) || "";

    var html = Markdown.getSanitizingConverter().makeHtml(markdown);

    $(element).html(html);
}

ko.bindingHandlers['markdown'] = { init: setMarkdown, update: setMarkdown };

ko.bindingHandlers.wrap = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var elt = $(element);

        elt.on('click', function () {
            var value = valueAccessor();
            var allBindings = allBindingsAccessor();

            var wrap = ko.utils.unwrapObservable(value);
            var target = allBindings.target;
            var targetProperty = allBindings.targetProperty;

            var targetElt = $(target);
            var targetEltUnwrapped = targetElt.get(0);
            var selectionStart = targetEltUnwrapped.selectionStart;
            var selectionEnd = targetEltUnwrapped.selectionEnd;

            var value = ko.utils.unwrapObservable(viewModel[targetProperty]);

            while ((value[selectionStart] == ' ' || value[selectionStart] == '\t') && selectionStart <= selectionEnd) {
                selectionStart++;
            }

            while ((value[selectionEnd - 1] == ' ' || value[selectionEnd - 1] == '\t') && selectionStart <= selectionEnd) {
                selectionEnd--;
            }

            if (value) {
                var newValue =
                    value.substring(0, selectionStart) +
                    wrap +
                    value.substring(selectionStart, selectionEnd) +
                    wrap +
                    value.substring(selectionEnd, value.length);

                viewModel[targetProperty](newValue);

                targetEltUnwrapped.selectionStart = selectionStart + wrap.length;
                targetEltUnwrapped.selectionEnd = selectionEnd + wrap.length;
            } else {
                viewModel[targetProperty](wrap + wrap);

                targetEltUnwrapped.selectionStart = targetEltUnwrapped.selectionEnd = wrap.length;
            }
        });
    }
};


ko.bindingHandlers.prepend = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var elt = $(element);

        elt.on('click', function () {
            var value = valueAccessor();
            var allBindings = allBindingsAccessor();

            var prepend = ko.utils.unwrapObservable(value);
            var target = allBindings.target;
            var targetProperty = allBindings.targetProperty;

            var targetElt = $(target);
            var targetEltUnwrapped = targetElt.get(0);
            var selectionStart = targetEltUnwrapped.selectionStart;
            var selectionEnd = targetEltUnwrapped.selectionEnd;

            var value = ko.utils.unwrapObservable(viewModel[targetProperty]);

            while (selectionStart > 0 && value[selectionStart] != '\n') {
                selectionStart--;
            }

            if (value[selectionStart] == '\n') {
                selectionStart++;
            }

            while (selectionEnd <= value.length && value[selectionEnd - 1] != '\n') {
                selectionEnd++;
            }

            if (value[selectionEnd - 1] == '\n') {
                selectionEnd--;
            }

            if (value) {
                var lines = value.substring(selectionStart, selectionEnd).split('\n');

                var newValue = value.substring(0, selectionStart);

                newValue += '\n';

                for (var i = 0 ; i < lines.length; i++) {
                    newValue += prepend;
                    newValue += lines[i];
                    newValue += '\n';
                }

                newValue += value.substring(selectionEnd, value.length);

                viewModel[targetProperty](newValue);

                targetEltUnwrapped.selectionStart = targetEltUnwrapped.selectionEnd = selectionStart + prepend.length + 1;
            } else {
                viewModel[targetProperty](prepend);

                targetEltUnwrapped.selectionStart = targetEltUnwrapped.selectionEnd = prepend.length;
            }
        });
    }
};