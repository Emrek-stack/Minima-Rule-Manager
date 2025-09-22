var destinationEventsManager = {
    events: [],
    isRelation: function (element, container) {
        if (this.events.length == 0)
            return false;
        return this.events.filter(function (item, i) { return item.relation == element && item.container == container; }).length > 0;
    }

};

; (function () {
    "use strict";
    function setup($) {
        var defaults = {
            controller: 'DestinationPluginData',
            action: 'GetDestinationModal',
            method: 'POST',
            filter: null,
            container: "body",
            id: null,
            validate: false,
            minInput: 3,
            closeOnSelect: false,
            fullSearch: true,
            defaultLabel: "Seçiniz",
            valueField: "DestinationSourceId",
            textField: "Name",
            filterType: null,
            dataType: "json",
            isSingle: false,
            validation: false,
            afterReload: false,
            text: false,
            reinit: false,
            destinationType: false,
            destinationTypeElement: false,
            removeData: true,
            type: 'autocomplete',
            eventName: "reload",
            customDropdown: '',
            afterChange: function (response, otions) {
                return true;
            },
            onError: function (data) {
            },
        };
        $.fn.destination = function (opts) {
            this.each(function () {
                var tempDefault = defaults;
                var $el = $(this);
                var fullOpts = $.extend({}, tempDefault, opts || $el.data());
                install($el, fullOpts);
            });
        };
        $.destinationBinderOption = function (opts) { };
        function destinationType() {
            return {
                Airport: 1,
                City: 2,
                Hotel: 3,
                Area: 4,
                Region: 5,
                Country: 6,
                Continent: 7,
                District: 8,
                DeparturePoint: 9

            };
        }
        function pluginType() {
            return {
                autocomplete: installAutocomplete,
                selectbox: installSelectBox,
                modal: installModal,
            };
        }
        function hasBindPlugin(el) {
            return el.data("plugins") == "destination-binder";
        }
        function removeData(el) {
            var dataKeys = Object.keys(defaults);
            $.each(dataKeys, function (index, item) {
                el.removeAttr("data-" + item)
                el.removeData(item);
            });
        }
        function install(el, opts) {
            if (!opts.reinit && hasBindPlugin(el))
                return;
            if (!opts.isSingle) {
                el.addClass("js-example-theme-multiple");
                el.parent().parent().addClass("grd-form-group-lg");
            } else {
                el.addClass("js-example-theme-single")
            }
            if (typeof pluginType()[opts.type] == "function") {
                pluginType()[opts.type](el, opts);
            }
            if (opts.removeData)
                removeData(el);
            el.data({ plugins: "destination-binder" });
        }
        function prepareData(el, opts) {
            var postData = {
                elementId: opts.id,
                isSingle: opts.isSingle,
                IsValid: true,
                fullSearch: opts.fullSearch,
                isBootstrap: window["bootstrapPlugins"]
            };
            if (opts.filter) {
                if (typeof opts.filter === "string") {
                    opts.filter = $.parseJSON(opts.filter);
                }
                var isValidData = function (value) {
                    return !(value == undefined || value == 0 || !value || value == "" || opts.defaultLabel == value)
                }
                var filterKeys = Object.keys(opts.filter);
                var filterData = {};
                $.each(filterKeys, function (index, item) {
                    filterData[item] = [];
                    $.each(opts.filter[item], function (i, filterItem) {
                        var value = filterItem;
                        if (filterItem) {
                            if (filterItem.toString().indexOf("#") > -1) {
                                value = $(filterItem).val();
                                if (typeof value == "object" && value) {
                                    $.each(value, function (i, valueItem) {
                                        if (!isValidData(valueItem)) {
                                            postData.IsValid = false;
                                            return false;
                                        } else {
                                            filterData[item].push(valueItem);
                                        }
                                    })
                                } else {
                                    if (!isValidData(value)) {
                                        console.log(el.attr("id") + " not valid")
                                        postData.IsValid = false;
                                        return false;
                                    }
                                    else {
                                        filterData[item].push(value);
                                    }
                                }
                            } else {
                                filterData[item].push(value);
                            }
                        }
                    })
                });
                postData.filterValues = JSON.stringify(filterData);
            }
            if (opts.filterType)
                postData.destinationTypes = opts.filterType;
            if (opts.isSingle)
                postData.selectedValues = $(el).val();
            else {
                if ($(el).val() && $(el).val().length > 0)
                    postData.selectedValues = $(el).val().join(',');

            }
            return postData;

        }
        function bindEvent(el, eventName, callBack) {
            el.bind(eventName, function () {
                if (callBack)
                    callBack();
            });
        };
        function installAutocomplete(el, opts) {
            updateElement(el, opts);
        }
        function installSelectBox(el, opts) {

            if (opts.afterReload) {
                $.each(opts.afterReload.split(','), function (i, item) {
                    destinationEventsManager.events.push({
                        el: el,
                        relation: item,
                        container: opts.container,
                    })
                });
            }
            updateElement(el, opts);
        }
        function installModal(el, opts) {
            var destinationHandler = el.next();
            updateElement(el, opts);
            destinationHandler.bind("click", function () {
                if (!el.attr("disabled")) {
                    var postData = prepareData(el, opts);
                    ajaxMethods.getHtmlData({
                        controller: opts.controller,
                        action: opts.action,
                        element: grd.utils.destinationModalSelector,
                        value: postData,
                        isModal: true,
                        onComplete: function (resultHtml) {
                            if (resultHtml) {
                                var html = {};
                                if (typeof resultHtml == "object") {
                                    html = $(resultHtml.Result);
                                }
                                var button = $("#DestinationSelectButton");
                                if (button && button.length > 0) {
                                    $(button).bind("click", function () {
                                        var tableObject = datatableObject.datatables.filter(function (item) { return item.content.selector == ("#destinationSelectContainer" + opts.id) })[0];
                                        var selectedValues = [];
                                        tableObject.table.$('tr').each(function (index, rowhtml) {
                                            if (opts.isSingle) {
                                                var checkedRadio = $('input[type="radio"]:checked', rowhtml);
                                                if (checkedRadio.length == 1) {
                                                    selectedValues.push({ Value: $(checkedRadio).data("value"), Text: $(checkedRadio).data("text") });
                                                }
                                            } else {
                                                var checkedCheckbox = $('input[type="checkbox"]:checked', rowhtml);
                                                if (checkedCheckbox.length == 1) {
                                                    selectedValues.push({ Value: $(checkedCheckbox).data("value"), Text: $(checkedCheckbox).data("text") });
                                                }
                                            }
                                        });
                                        $("option", el).remove();
                                        if (selectedValues.length > 0) {
                                            for (var i = 0; i < selectedValues.length; i++) {
                                                var item = selectedValues[i];
                                                $(el).append("<option value='" + item.Value + "' selected='selected'>" + item.Text + "</option>");
                                            }
                                        }
                                        if (postData.isBootstrap)
                                            grd.utils.closeModal(grd.utils.destinationModalSelector);
                                        else
                                            shared.closeModal();
                                        triggerPlugin(el, opts);


                                    });
                                }

                            }
                        }
                    });
                }
            });
        }
        function updateElement(el, opts) {
            function getDestinationIcon(type) {

            }
            function capitalizeFirstLetter(string) {
                return string.charAt(0).toUpperCase() + string.slice(1).toLowerCase();
            }
            function getDestinationUrl(pathValue) {
                var resultValue = null;
                if (pathValue) {
                    var list = pathValue.split('|');
                    resultValue = "";
                    for (var i = list.length - 2; i > -1; i--) {
                        resultValue += list[i] + ", ";
                    }
                    return resultValue.substring(0, resultValue.length - 2);
                }
                return resultValue;
            }
            function formatDestination(dataItem) {
                if (dataItem.id == dataItem.text || dataItem.text == opts.defaultLabel || dataItem.loading) return false;
                var item = $("<div class='autocomplete-menu-item'></div>");
                var link = $("<a class='ui-corner-all'></a>");
                var className = (dataItem && dataItem.DestinationTypeName) ? dataItem.DestinationTypeName.toLowerCase() : "";
                var iconItem = $('<i class="autocomplete-icons autocomplete-' + className + '"></i>')

                var desItem = $("<em>" + dataItem.Name + "</em><span>" + getDestinationUrl(dataItem.PathValue) + "</span>");
                link.append(iconItem).append(desItem);
                item.append(link);
                return item;
            }
            function formatDestinationSelection(item, element) {
                var text = [item.text];
                // if(opts.text  && item) 
                //   opts.text.split(",").forEach(function(i){ 
                //       var key=capitalizeFirstLetter(i)+"Name";
                //      item[key]?text.push(item[key]):false;
                //   })
                if (item["DestinationTypeId"]) {
                    if (item.DestinationTypeId == destinationType().Airport) {
                        text.push(item.CityName);
                        text.push(item.CountryName);
                    }
                    else if (item.DestinationTypeId == destinationType().Area) {
                        text.push(item.DistrictName);
                        text.push(item.CityName);
                    }
                    else if (item.DestinationTypeId == destinationType().District) {
                        text.push(item.CityName);
                    }
                }

                var typeElement = false;

                if (el.prev().is("input")) {
                    typeElement = $(el).prev();
                } else if (!typeElement && opts.destinationTypeElement) {
                    typeElement = $(opts.destinationTypeElement);
                }
                if (typeElement) {
                    if (item.DestinationTypeName) {
                        if (opts.destinationType)
                            $(typeElement).val(opts.destinationType.replace("{0}", item.DestinationTypeName))
                        else
                            $(typeElement).val(item.DestinationTypeName)
                    }
                }
                return text.join(", ") || "[" + item.id + "]";
            }

            if (el) {
                $(el).parent().addClass("destination-modal-parent destination-plugin-parent")
                $(el).parent().attr("id", $(el).attr("id") + "_Parent")
                if (opts.type == "modal") {
                    var destinationElement = $(el).select2({
                        placeholder: {
                            id: 0,
                            text: opts.defaultLabel
                        },
                        tags: !opts.isSingle,
                        minimumResultsForSearch: -1,
                        allowClear: opts.isSingle,
                        containerCssClass: "destination-modal-container destination-plugin-container"
                    });
                    destinationElement.on("select2:open", function (e) {
                        destinationElement.select2("close");
                    })
                }
                else if (opts.type == "autocomplete") {
                    var selectedValue = $(el).val();
                    var ajaxRequest = null;
                    ajaxRequest = {
                        url: AppBaseUrl + "/" + opts.controller + "/GetDestinationAutocomplete",
                        dataType: 'json',
                        method: 'POST',
                        delay: 250,
                        beforeSend: function (xhr) {
                            var data = prepareData(el, opts);
                            if (!data)
                                xhr.abort();
                            return xhr;
                        },
                        data: function (params) {
                            var data = prepareData(el, opts);
                            if (!data) return false;
                            data.term = params.term;
                            return data;
                        },
                        processResults: function (data, params) {
                            params.page = params.page || 1;
                            return {
                                results: $.map(data, function (obj) {
                                    obj["id"] = obj[opts.valueField];
                                    obj["text"] = obj[opts.textField];
                                    return obj
                                }),
                                pagination: {
                                    more: (params.page * 30) < data.total_count
                                }
                            };
                        },
                        cache: true
                    };
                    var instance = $(el).select2({
                        placeholder: {
                            id: 0,
                            text: opts.defaultLabel
                        },
                        closeOnSelect: (!opts.closeOnSelect && opts.isSingle),
                        dropdownAutoWidth: 'true',
                        tags: !opts.isSingle,
                        allowClear: opts.isSingle,
                        containerCssClass: "destination-autocomplete-container destination-plugin-container",
                        dropdownCssClass: "destination-drop" + " " + opts.customDropdown,
                        data: selectedValue,
                        ajax: ajaxRequest,
                        escapeMarkup: function (markup) { return markup; },
                        templateResult: formatDestination,
                        templateSelection: formatDestinationSelection,

                        minimumInputLength: opts.minInput,
                    });
                    instance.val(selectedValue).trigger("change");
                }
                else if (opts.type == "selectbox") {
                    var selectedValue = $(el).data("selected", $(el).val());
                    var initSelectbox = function (el, pluginData) {
                        pluginData = pluginData || [];
                        var isReInit = false;
                        if ($(el).data("select2")) {
                            $("option", $(el)).remove();
                            $(el).select2("destroy");
                            isReInit = true;
                        }
                        if (!$(el).data("selected"))
                            pluginData.unshift({ text: opts.defaultLabel, id: "" })
                        var instance = $(el).select2({
                            placeholder: {
                                id: "",
                                text: opts.defaultLabel
                            },
                            closeOnSelect: true,
                            data: pluginData,
                            allowClear: opts.isSingle,
                            containerCssClass: "destination-selectbox-container destination-plugin-container",
                            dropdownCssClass: "destination-drop",
                            escapeMarkup: function (markup) { return markup; },
                            templateResult: formatDestination,
                            templateSelection: formatDestinationSelection,

                        });
                        if ($(el).data("selected") && isReInit)
                            $(el).val($(el).data("selected"))
                        if (isReInit) {
                            instance.trigger("change");
                            $(el).data("selected", false)
                        }
                        return instance;
                    }
                    var instance = initSelectbox(el, [], opts);

                    instance.on("change", function (e) {

                        triggerPlugin(el, opts);
                    })
                    instance.unbind("relaod");
                    instance.bind("reload", function (e) {
                        $("option", $(el)).remove();
                        getData($(this));
                    });
                    var getData = function (element) {
                        var postData = prepareData(element, opts);
                        if (postData.IsValid) {
                            ajaxMethods.sendPostRequest({
                                controller: opts.controller,
                                action: "GetDestinationSelectBox",
                                value: postData,
                                //async: false,
                                block: { element: "#" + element.parent().attr("id"), textOnly: true },
                                onComplete: function (resultData) {
                                    //console.log("#"+element.attr("id")+" elementi yeniden yükleniyor")
                                    if (resultData) {
                                        var pluginData = $.map(resultData, function (obj) {
                                            obj["id"] = obj[opts.valueField];
                                            obj["text"] = obj[opts.textField];
                                            return obj
                                        })
                                        initSelectbox(element, pluginData);
                                    }
                                }
                            });
                        }
                    }


                    if (!destinationEventsManager.isRelation("#" + el.attr("id"), opts.container))
                        getData(el);
                }
            }

        }
        function triggerPlugin(el, opts) {
            if (opts.afterReload) {
                var element = opts.afterReload.split(',');
                $.each(element, function (index, item) {
                    if ($(item).data("html-loader")) {
                        $(item).trigger("reload");
                    } else if (hasBindPlugin($(item))) {
                        $(item).val('').trigger("change")
                        $(item).trigger("reload");
                    }
                })
            } else if (opts.afterChange) {
                if (typeof opts.afterChange == "string") {
                    grd.utils.executeFunctionByName(opts.afterLoad, [$(el).val(), opts]);
                }

            }
        }
    }

    if (typeof define === 'function' && define.amd && define.amd.jQuery) {
        define(['jquery'], setup);
    } else {
        setup(jQuery);
    }
})();