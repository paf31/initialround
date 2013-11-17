ko.observableArray.fn.select = function (selector) {
    return ko.computed(function () {
        var items = this();
        var copy = [];

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            copy.push(selector(item));
        }

        return copy;
    }, this);
}

ko.observableArray.fn.sortBy = function (keySelector) {
    return ko.computed(function () {
        var items = this();
        return _.sortBy(items, keySelector);
    }, this);
}

$.fn.centered = function () {
    $(this).each(function () {
        var elt = $(this);

        var width = elt.outerWidth();
        var height = elt.outerHeight();
        var parentWidth = elt.parent().outerWidth();
        var parentHeight = elt.parent().outerHeight();

        var left = (parentWidth - width) / 2;
        var top = (parentHeight - height) / 2;

        elt.css('position', 'absolute');
        elt.css('margin-left', left + 'px');
        elt.css('margin-top', top + 'px');
    });
};

ko.bindingHandlers.popup = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        $(element).css('display', 'block');

        $(window).on('resize', function () {
            $(element).children().centered();
        });

        if (valueUnwrapped) {
            $(element).show();
            $(element).children().centered();
        } else {
            $(element).hide();
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        if (valueUnwrapped) {
            $(element).show();
            $(element).children().centered();
        } else {
            $(element).hide();
        }
    }
};