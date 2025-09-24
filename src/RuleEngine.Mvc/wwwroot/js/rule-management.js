var isStaticRule = false;

// Parameter Manager - Base class for all parameter types
var ParameterManager = {};

// Base Parameter class
var Parameter = function (data) {
    var _this = this;
    _this.TypeName = data.TypeName || "Parameter";
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "Parameter";
    _this.ElementId = "param_" + RuleUtility.GenerateString();
    
    _this.ValidateProp = function (prop) {
        return prop && prop.toString().trim().length > 0;
    };
    
    _this.ValidateSelected = function () {
        return true; // Override in derived classes
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: [] };
    };
};

// Numeric Parameter
var NumericParameter = function (data) {
    data.TypeName = data.TypeName || "NumericParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "NumericParameter";
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected ? _this.Selected() : "" };
    };
};
ParameterManager.NumericParameter = NumericParameter;

// String Parameter
var StringParameter = function (data) {
    data.TypeName = data.TypeName || "StringParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "StringParameter";
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected ? _this.Selected() : "" };
    };
};
ParameterManager.StringParameter = StringParameter;

// DateTime Parameter
var DateTimeParameter = function (data) {
    data.TypeName = data.TypeName || "DateTimeParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "DateTimeParameter";
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected ? _this.Selected() : "" };
    };
};
ParameterManager.DateTimeParameter = DateTimeParameter;

// Array Parameter
var ArrayParameter = function (data) {
    data.TypeName = data.TypeName || "ArrayParameter";
    var _this = this;
    StringParameter.call(_this, data);
    _this.ArrayType = data.ArrayType || "System.Int32";
    _this.Name = data.Name || "ArrayParameter";
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Data = ko.observableArray(data.Data || []);
    _this.Selected = ko.observableArray(data.Selected || []);
    
    _this.Validate = function (index) {
        if (!_this.ValidateProp(_this.ArrayType)) {
            console.error("Lütfen type bilgisini giriniz", index + ". parametre uyarı");
            return false;
        } else if (!_this.ValidateProp(_this.Title)) {
            console.error("Lütfen başlık bilgisini giriniz", index + ". parametre uyarı");
            return false;
        }
        return true;
    };
    
    _this.Add = function () {
        _this.Data.push({ Key: '', Value: '' });
    };
    
    _this.Remove = function (item) {
        _this.Data.remove(item);
    };
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.ArrayParameter = ArrayParameter;

// List Parameter
var ListParameter = function (data) {
    data.TypeName = data.TypeName || "ListParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "ListParameter";
    _this.Items = ko.observableArray(data.Items || []);
    _this.Selected = ko.observable(data.Selected || "");
    
    _this.Add = function () {
        _this.Items.push({ Title: '', Description: '', ExpressionFormat: '', Value: '' });
    };
    
    _this.Remove = function (item) {
        _this.Items.remove(item);
    };
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.ListParameter = ListParameter;

// Definition Parameter
var DefinitionParameter = function (data) {
    data.TypeName = data.TypeName || "DefinitionParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "DefinitionParameter";
    _this.DefinitionType = data.DefinitionType || "";
    _this.ArrayType = data.ArrayType || "";
    _this.Selected = ko.observableArray(data.Selected || []);
    _this.DefinitionAllTypes = ko.observableArray(window.definitionTypes || []);
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.DefinitionParameter = DefinitionParameter;

// Destination Parameter
var DestinationParameter = function (data) {
    data.TypeName = data.TypeName || "DestinationParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "DestinationParameter";
    _this.Filter = data.Filter || "";
    _this.DestinationTypes = ko.observableArray(data.DestinationTypes || []);
    _this.Selected = ko.observableArray(data.Selected || []);
    _this.DestinationAllTypes = ko.observableArray(window.destinationTypes || []);
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.DestinationParameter = DestinationParameter;

// Readonly Parameter
var ReadonlyParameter = function (data) {
    data.TypeName = data.TypeName || "ReadonlyParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "ReadonlyParameter";
    _this.ReadonlyValue = ko.observable(data.ReadonlyValue || "");
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.ReadonlyValue() };
    };
};
ParameterManager.ReadonlyParameter = ReadonlyParameter;

// Boolean List Parameter
var BooleanListParameter = function (data) {
    data.TypeName = data.TypeName || "BooleanListParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "BooleanListParameter";
    _this.Selected = ko.observable(data.Selected || "");
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.BooleanListParameter = BooleanListParameter;

// Equality List Parameter
var EqualityListParameter = function (data) {
    data.TypeName = data.TypeName || "EqualityListParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "EqualityListParameter";
    _this.Symbol = ko.observable(data.Symbol || false);
    _this.Operators = ko.observableArray(data.Operators || []);
    _this.Selected = ko.observable(data.Selected || "");
    _this.OperatorList = ko.observableArray([
        { Text: "Eşittir", Value: "==" },
        { Text: "Eşit Değildir", Value: "!=" },
        { Text: "Büyüktür", Value: ">" },
        { Text: "Küçüktür", Value: "<" },
        { Text: "Büyük Eşittir", Value: ">=" },
        { Text: "Küçük Eşittir", Value: "<=" }
    ]);
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: _this.Selected() };
    };
};
ParameterManager.EqualityListParameter = EqualityListParameter;

// DateTime Group Parameter
var DateTimeGroupParameter = function (data) {
    data.TypeName = data.TypeName || "DateTimeGroupParameter";
    var _this = this;
    Parameter.call(_this, data);
    _this.Title = data.Title || "";
    _this.Description = data.Description || "";
    _this.Name = data.Name || "DateTimeGroupParameter";
    _this.FirstValue = ko.observable(data.FirstValue || "");
    _this.SecondValue = ko.observable(data.SecondValue || "");
    
    _this.ValidateForRule = function () {
        return _this.ValidateSelected();
    };
    
    _this.SerializeForRule = function () {
        return { Parameters: [_this.FirstValue(), _this.SecondValue()] };
    };
};
ParameterManager.DateTimeGroupParameter = DateTimeGroupParameter;

var RuleUtility = {
    GenerateString: function () {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < 5; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    },
    Factory: function (type, namespace, data) {
        try {
            if (!type)
                throw "type bilgisi alınamadı";
            var arguments = data || {};
            if (namespace[type])
                return new namespace[type](arguments);
            else
                throw type + " tipi tanımı bulunamadı"
        } catch (e) {
            console.error(e)
        }
    },
    TypeIsDefine: function (type, namespace) {
        return namespace[type] ? true : false;
    }
};
var MetadataManager = function (managerData) {
    var Metadata = (function (_super) {
        function Metadata(metadata) {
            var _this = this;
            console.log("Metadata constructor called with:", metadata);
            console.log("metadata.ParameterTypes:", metadata.ParameterTypes);
            console.log("metadata.CategoryItems:", metadata.CategoryItems);
            
            // Store original metadata before extend
            var originalParameterTypes = metadata.ParameterTypes;
            var originalCategoryItems = metadata.CategoryItems;
            var originalCategories = metadata.Categories;
            
            $.extend(_this, { Id: 0, Name: "", Title: "", Description: "", ExpressionString: "", IsPredicate: true, }, metadata);
            _this.ParameterTypeId = ko.observable();
            _this.Parameters = ko.observableArray([]);
            _this.ParameterTypes = ko.observableArray(originalParameterTypes || []);
            _this.Categories = ko.observableArray(originalCategories || []);
            _this.CategoryItems = ko.observableArray(originalCategoryItems || []);
            
            console.log("After initialization:");
            console.log("_this.ParameterTypes():", _this.ParameterTypes());
            console.log("_this.CategoryItems():", _this.CategoryItems());
            _this.AddParameter = function (item) {
                console.log("AddParameter called");
                console.log("ParameterTypeId value:", _this.ParameterTypeId());
                
                // If no parameter type is selected, default to NumericParameter
                if (_this.ParameterTypeId() == null || _this.ParameterTypeId() == "" || _this.ParameterTypeId() == 0) {
                    console.log("No parameter type selected, defaulting to NumericParameter");
                    _this.ParameterTypeId("NumericParameter");
                }
                
                console.log("Creating parameter with type:", _this.ParameterTypeId());
                var param = RuleUtility.Factory(_this.ParameterTypeId(), ParameterManager);
                console.log("Parameter created:", param);
                if (param) {
                    _this.Parameters.push(param);
                    console.log("Parameter added to array. Total parameters:", _this.Parameters().length);
                    _this.BindPlugins({ select: true, scroll: true, sort: true });
                } else {
                    console.log("Failed to create parameter");
                }
            };
            _this.AddFormatToEditor = function () {
                var session = ace.edit("editor").getSession();
                session.insert({
                    row: session.getLength(),
                    column: 0
                }, "\n" + "{0}");
            };
            _this.AddObjectListToEditor = function () {
                var session = ace.edit("editor").getSession();
                session.insert({
                    row: session.getLength(),
                    column: 0
                }, "\n" + "new HashSet<object>(new object[]{{ {0} }})");
            };
            _this.AddVariableToEditor = function () {
                var session = ace.edit("editor").getSession();
                session.insert({
                    row: session.getLength(),
                    column: 0
                }, "\n" + "@{0}");
            };
            _this.Serialize = function () {
                var data = $.parseJSON(ko.toJSON(_this));
                data.ParameterTypes = null;
                data.CategoryItems = null;
                data.Parameters.forEach(function (item) {
                    if (item["DefinitionAllTypes"])
                        item["DefinitionAllTypes"] = null;
                    if (item["DestinationAllTypes"])
                        item.DestinationAllTypes = null;
                });
                return data;
            };
            _this.RemoveParameter = function (item) {
                _this.Parameters.remove(item)
            };
            _this.UpdateSort = function (e) {
                e.preventDefault();
                _this.BindPlugins({ select: true });
            };
            _this.Save = function () {
                if (_this.Validate()) {
                    var data = _this.Serialize();
                    var editor = ace.edit("editor");
                    data.ExpressionString = editor.getValue();
                    
                    $.ajax({
                        url: '/RuleMetadata/Save',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(data),
                        success: function (result) {
                            if (result && result.isSuccess) {
                                _this.Id = result.id;
                                alert('Metadata başarıyla kaydedildi!');
                            } else {
                                alert('Hata: ' + (result.message || 'Bilinmeyen hata'));
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error("AJAX Error:", status, error);
                            alert('Hata: ' + error);
                        }
                    });
                }
            };
            _this.Validate = function () {
                var validatePropery = function (prop) {
                    return prop || prop.trim() != "";
                };

                if (!validatePropery(_this.Name)) {
                    alert("Lütfen metadata adını giriniz ");
                    return false;
                }
                else if (!validatePropery(ace.edit("editor").getValue())) {
                    alert("Lütfen expression giriniz");
                    return false;
                }
                var result = [];
                if (_this.Parameters() && _this.Parameters().length > 0) {
                    $.each(_this.Parameters(), function (i, item) {
                        if (item["Validate"]) {
                            if (!item["Validate"](i)) {
                                result.push(0);
                                return false;
                            }
                        }
                    })
                }
                return result.length < 1;
            };
            _this.BindPlugins = function (option) {
                //// Bind Plugins
                option = $.extend({ select: false, scroll: false, sort: false, editor: false }, option);
                if (option.select && typeof $.fn.select2 !== 'undefined') {
                    $(".custom-select2").select2({});
                }
                if (option.scroll) {
                    $("html, body").animate({ scrollTop: $(document).height() - $(window).height() });
                }
                if (option.sort) {

                } if (option.editor) {
                    var editor = ace.edit("editor");
                    editor.setTheme("ace/theme/monokai");
                    editor.session.setMode("ace/mode/csharp");
                    editor.getSession().on('change', function () {
                        $("#ExpressionStringText").val(editor.getSession().getValue());
                    });
                }
            };
            _this.BindParameters = (function (model) {
                var pm = new ParameterManager();
                if (model) {
                    $.each(model, function (i, item) {
                        _this.Parameters.push(RuleUtility.Factory(item.TypeName, ParameterManager, item));
                    });
                    _this.BindPlugins({ select: true });
                }
            })(metadata.Parameters);
        }
        return _super.Metadata = Metadata;
    })(MetadataManager);
    return new Metadata(managerData);
};
var ParameterManager = function (paramData) {

    var parameterManager = this;

    ///Abstract parameter
    var Parameter = (function (parameter) {
        function Parameter(data) {
            var _this = this;
            _this.TypeName = data["TypeName"];
            _this.Selected = ko.observableArray(data["Selected"] || []);
            _this.DisplayFormat = data.DisplayFormat || "{0}";
            _this.ElementId = RuleUtility.GenerateString();
            _this.ValidateProp = function (prop) {
                return prop && prop != null &&
                    ((prop.constructor === String &&
                        (prop.trim() != "" && prop.trim().length > 0)) ||
                        (prop.constructor === Array && prop.length > 0));

            };
            /// a=Validate edilecek olan değer
            _this.ValidateSelected = function (a) {
                return _this.ValidateProp(!a ? _this.Selected() : a);
            }
        }
        return Parameter;
    })(Parameter);

    /// Base Parameter
    var NumericParameter = (function (_super) {
        function NumericParameter(data) {
            data.TypeName = data.TypeName || "NumericParameter";
            var _this = this;
            Parameter.call(_this, data);
            _this.Title = data.Title;
            _this.Description = data.Description;
            _this.Name = data.Name || "NumericParameter";
            _this.Validate = function (index) {
                if (!_this.ValidateProp(_this.Title)) {
                    alert("Lütfen başlık bilgisini giriniz");
                    return false;
                }
                return true;
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            }

        }
        return _super.NumericParameter = NumericParameter;
    })(ParameterManager);

    /// String
    var StringParameter = (function (_super) {
        function StringParameter(data) {
            data.TypeName = data.TypeName || "StringParameter";
            var _this = this;
            Parameter.call(_this, data);
            _this.Title = data.Title;
            _this.Description = data.Description;
            _this.Name = data.Name || "StringParameter";
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            }
        }
        return _super.StringParameter = StringParameter;

    })(ParameterManager);

    ///Datetime
    var DateTimeParameter = (function (_super) {
        function DateTimeParameter(data) {
            data.TypeName = data.TypeName || "DateTimeParameter";
            var _this = this;
            Parameter.call(_this, data);
            _this.Title = data.Title;
            _this.Description = data.Description;
            _this.Name = data.Name || "DateTimeParameter";
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            }

        }
        return _super.DateTimeParameter = DateTimeParameter;
    })(ParameterManager);

    ///Array
    var ArrayParameter = (function (_super) {
        function ArrayParameter(data) {
            data.TypeName = data.TypeName || "ArrayParameter";
            var _this = this;
            StringParameter.call(_this, data);
            _this.ArrayType = data.ArrayType || "System.Int32";
            _this.Name = data.Name || "ArrayParameter";
            _this.Title = data.Title;
            _this.PluginParameters = ko.observableArray(Object.keys(ko.bindingHandlers.MetadataArrayParameterBinder ? ko.bindingHandlers.MetadataArrayParameterBinder.options : {}));
            _this.Description = data.Description;
            _this.Data = ko.observableArray([]);
            _this.Validate = function (index) {
                if (!_this.ValidateProp(_this.ArrayType)) {
                    alert("Lütfen type bilgisini giriniz");
                    return false;
                } else if (!_this.ValidateProp(_this.Title)) {
                    alert("Lütfen başlık bilgisini giriniz");
                    return false;
                }
                return true;
            };
            _this.Add = function () {
                _this.Data.push({ Key: '', Value: '' });
                $(".custom-select2").select2();
            };
            _this.Remove = function (item) {
                _this.Data.remove(item)
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function (index) {
                var result = { Parameters: [] };
                result.Parameters = $("#" + _this.ElementId).val();
                var options = $.map($("#" + _this.ElementId).children("option").filter(":selected"), function (item) {
                    return $(item).text();
                });
                if (options && options.length > 0)
                    result.ParameterLabels = { Key: index, Value: options };
                return result;
            };
            _this.BindData = (function (dataList) {
                if (dataList) {
                    _this.ElementTemplate = ko.observable("Multiple");
                    if (dataList.Data) {
                        _this.ElementTemplate = ko.observable(dataList.Data["isMultiple"] == false ? "Single" : "Multiple");
                        Object.keys(dataList.Data).forEach(function (item) { _this.Data.push({ Key: item, Value: dataList.Data[item] }); });
                    }
                }
            })(data);

        }
        return _super.ArrayParameter = ArrayParameter;
    })(ParameterManager);

    ///DatetimeGroup
    var DateTimeGroupParameter = (function (_super) {
        function DateTimeGroupParameter(data) {
            data.TypeName = data.TypeName || "DateTimeGroupParameter";
            var _this = this;
            Parameter.call(_this, data);
            _this.Title = data.Title;
            _this.FirstValue = ko.observable("");
            _this.SecondValue = ko.observable("");
            _this.Description = data.Description;
            _this.Name = data.Name || "DateTimeGroupParameter";
            _this.ValidateForRule = function () {
                return _this.ValidateProp(_this.FirstValue()) && _this.ValidateProp(_this.SecondValue());
            };
            _this.SerializeForRule = function () {
                return { Parameters: [_this.FirstValue(), _this.SecondValue()] };
            };
            var bindData = (function (dataList) {
                if (dataList && dataList.Selected) {
                    _this.FirstValue(dataList.Selected[0].split('T')[0] + "\"" || "");
                    _this.SecondValue(dataList.Selected[0].split('T')[0] + "\"" || "");
                }
            })(data);
        }
        return _super.DateTimeGroupParameter = DateTimeGroupParameter;
    })(ParameterManager);

    /// Readonly
    var ReadonlyParameter = (function (_super) {
        function ReadonlyParameter(data) {
            data.TypeName = data.TypeName || "ReadonlyParameter";
            var _this = this;
            Parameter.call(_this, data);
            _this.Name = data.Name || "ReadonlyParameter";
            _this.ReadonlyValue = data.ReadonlyValue;
            _this.Validate = function (index) {
                if (!_this.ValidateProp(_this.ReadonlyValue)) {
                    alert("Lütfen readonly parametresi değerini giriniz");
                    return false;
                }
                return true;
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected(_this.ReadonlyValue);
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.ReadonlyValue };
            }
        }
        return _super.ReadonlyParameter = ReadonlyParameter;
    })(ParameterManager);

    /// List
    var ListParameter = (function (_super) {
        function ListParameter(data) {
            data.TypeName = data.TypeName || "ListParameter";
            var _this = this;
            NumericParameter.call(_this, data);
            _this.Name = data.Name || "ListParameter";
            _this.Title = data.Title;
            _this.Description = data.Description;
            _this.Items = ko.observableArray([]);
            _this.Add = function () {
                _this.Items.push({ Title: "", Description: "", ExpressionFormat: "{0}", Value: "" });
            };
            _this.Remove = function (item) {
                _this.Items.remove(item);
            };
            _this.Validate = function (index) {
                var result = [];
                $.each(_this.Items(), function (i, item) {
                    if (!_this.ValidateProp(item.Value)) {
                        result.push(0);
                        alert("Lütfen list parameter value değerini giriniz");
                        return;
                    }
                });

                return result.length < 1;
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            };
            _this.BindData = (function (listData) {
                if (listData.Items) {
                    if (listData.Items.constructor === Object) {
                        _this.Items(Object.keys(listData.Items).map(function (key) { return $.extend(listData.Items[key], { Name: "" }, { Name: key }) }))
                    } else if (listData.Items.constructor === Array) {
                        _this.Items(listData.Items);
                    }
                }
            })(data)
        }
        return _super.ListParameter = ListParameter;
    })(ParameterManager);

    /// Definition
    var DefinitionParameter = (function (_super) {
        function DefinitionParameter(data) {
            data.TypeName = data.TypeName || "DefinitionParameter";
            var _this = this;
            ArrayParameter.call(_this, data);
            _this.Name = data.Name || "DefinitionParameter";
            _this.DefinitionType = ko.observable(data.DefinitionType || "");
            _this.DefinitionAllTypes = ko.observableArray([]);
            _this.Validate = function (index) {
                var result = [];
                if (!_this.DefinitionType() || _this.DefinitionType().length < 1) {
                    alert("Lütfen definition type giriniz");
                    result.push(0);
                }

                return result.length < 1;
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            };
            _this.BindData = (function (paramData) {
                if (window["definitionTypes"]) {
                    _this.DefinitionAllTypes(definitionTypes.map(function (i) { return { Text: i.split("Definition")[i.split("Definition").length - 1] == "Definition" ? i.split("Definition")[0] : i, Value: i }; }));
                } else {
                    // Mock data for demo
                    _this.DefinitionAllTypes([
                        { Text: "Customer", Value: "CustomerDefinition" },
                        { Text: "Product", Value: "ProductDefinition" },
                        { Text: "Order", Value: "OrderDefinition" }
                    ]);
                }
            })(data);
        }
        return _super.DefinitionParameter = DefinitionParameter;
    })(ParameterManager);

    /// Destination
    var DestinationParameter = (function (_super) {
        function DestinationParameter(data) {
            data.TypeName = data.TypeName || "DestinationParameter";
            var _this = this;
            ArrayParameter.call(_this, data);
            _this.Name = data.Name || "DestinationParameter";
            _this.DestinationAllTypes = ko.observableArray([]);
            _this.Items = ko.observableArray([]);
            _this.DestinationTypes = ko.observableArray(data.DestinationTypes || []);
            _this.Filter = ko.observable(data.Filter || "{}");
            _this.Validate = function (index) {
                var result = [];
                if (!_this.DestinationTypes() || _this.DestinationTypes().length < 1) {
                    alert("Lütfen destination type seçiniz");
                    result.push(0);
                }
                try {
                    var r = $.parseJSON(JSON.stringify(_this.Filter()));
                } catch (e) {
                    alert("Girilen filtre uygun formatta değil.");
                    result.push(0);
                }

                return result.length < 1;
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function (index) {
                var result = { Parameters: _this.Selected().join(",") };
                var options = $.map($("#" + _this.ElementId).children("option").filter(":selected"), function (item) {
                    return $(item).text();
                });
                if (options && options.length > 0)
                    result.ParameterLabels = { Key: index, Value: options };
                return result;
            };
            _this.BindData = (function (paramData) {
                if (window["destinationTypes"]) {
                    _this.DestinationAllTypes(Object.keys(destinationTypes).map(function (i) { return { Text: destinationTypes[i], Value: destinationTypes[i] }; }));
                }
                else if (paramData["ParameterLabels"]) {
                    var i = 0, items = [];
                    paramData["ParameterLabels"].forEach(function (k) { items.push({ Text: k, Value: _this.Selected()[i++] }); });
                    _this.Items(items);
                }
            })(data);
        }
        return _super.DestinationParameter = DestinationParameter;
    })(ParameterManager);

    //// EqualityList
    var EqualityListParameter = (function (_super) {
        function EqualityListParameter(data) {
            data.TypeName = data.TypeName || "EqualityListParameter";
            var _this = this;
            ListParameter.call(_this, data);
            var GetOperators = function () {
                var result = [
                    { Value: "==", Text: "Eşittir" },
                    { Value: "!=", Text: "Eşit Değildir" },
                    { Value: "<", Text: "Küçüktür" },
                    { Value: ">", Text: "Büyüktür" },
                    { Value: "<=", Text: "Küçük Eşittir" },
                    { Value: ">=", Text: "Büyük Eşittir" }
                ];
                return result;
            };
            _this.Name = data.Name || "EqualityListParameter";
            _this.OperatorList = ko.observableArray(GetOperators());
            _this.Operators = ko.observableArray(data.Operators);
            _this.Symbol = data.Symbol;
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            };
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            }

        }
        return _super.EqualityListParameter = EqualityListParameter;
    })(ParameterManager);

    //// BooleanList
    var BooleanListParameter = (function (_super) {
        function BooleanListParameter(data) {
            data.TypeName = data.TypeName || "BooleanListParameter";
            var _this = this;
            ListParameter.call(_this, data);
            _this.Name = data.Name || "BooleanListParameter";
            _this.SerializeForRule = function () {
                return { Parameters: _this.Selected() };
            };
            _this.ValidateForRule = function () {
                return _this.ValidateSelected();
            }
        }
        return _super.BooleanListParameter = BooleanListParameter;
    })(ParameterManager);

    return ParameterManager;
};
var StatementManager = function () {
    var statementManager = this;
    var Statement = (function (_super) {
        function Statement(statement) {
            var _this = this;
            _this.HasFocus = ko.observable(true);
            _this.ElementId = RuleUtility.GenerateString();
            _this.Passive = function () {
                _this.HasFocus(false);
            };
            _this.GetTypeName = function () {
                var funcNameRegex = /function (.{1,})\(/;
                var results = (funcNameRegex).exec((this).constructor.toString());
                return (results && results.length > 1) ? results[1] : "";
            };
            _this.FilterClass = function () {
                if (_this.GetTypeName() == "AndOrStatement")
                    return "ignore-group";
                return "";
            };
            _this.TemplateName = _this.GetTypeName();
            _this.Type = _this.GetTypeName();
        }
        return Statement;
    })(StatementManager);
    var AndOrStatement = (function (_super) {
        function AndOrStatement(statement) {
            var _this = this;
            Statement.call(_this, {});
            _this.Items = ko.observableArray([{ Title: "Ve", Name: "&&", Type: "AndOperatorStatement" }, { Title: "Veya", Name: "||", Type: "OrOperatorStatement" }]);
            _this.Selected = ko.observable(statement.Selected || "&&");
            _this.GetType = function (operator) {
                return _this.Items().filter(function (i) { return (i.Name === operator); })[0].Type || "AndOperatorStatement";
            };
            var BindData = (function (s) { if (s.Title === "VEYA") { _this.Selected("||"); _this.Type = "" } })(statement);
            _this.Type = _this.GetType(_this.Selected());
            _this.Serialize = function () {
                var jsonData = $.parseJSON(ko.toJSON(_this));
                jsonData.Parameters = [_this.Selected()];
                return jsonData;
            };
            _this.Validate = function () {
                return !(!_this.Selected() || _this.Selected().length < 1);

            };
            _this.Change = function () {
                _this.Type = _this.GetType(_this.Selected())
            }

        }
        _super.AndOperatorStatement = AndOrStatement;
        _super.OrOperatorStatement = AndOrStatement;
        return _super.AndOrStatement = AndOrStatement;
    })(StatementManager);
    var NamedRuleStatement = (function (_super) {
        function NamedRuleStatement(data) {
            var _this = this;
            Statement.call(_this, {});
            _this.StatementKey = "Statement_" + RuleUtility.GenerateString();
            _this.Name = data.Name || "";
            _this.Parameters = ko.observableArray([]);
            _this.Serialize = function () {
                var jsonData = $.parseJSON(ko.toJSON(_this));
                var i = 0;
                var p = $.map(_this.Parameters(), function (item) { return item.SerializeForRule(i++); });
                jsonData.Parameters = [];
                jsonData.ParameterLabels = [];
                p.forEach(function (item) {
                    if (item["Parameters"]) {
                        if (item.Parameters.constructor === Array)
                            jsonData.Parameters.push(item.Parameters.join(","));
                        else
                            jsonData.Parameters.push(item.Parameters);
                    }
                    if (item["ParameterLabels"]) jsonData.ParameterLabels.push(item.ParameterLabels);
                });

                return jsonData;
            };
            _this.Validate = function () {
                var result = [];
                if (!_this.MetadataId() || _this.MetadataId() < 0) {
                    alert("Lütfen koşul seçiniz");
                    return false;
                }
                if (_this.Parameters && _this.Parameters().length > 0) {
                    _this.Parameters().forEach(function (item) {
                        if (!item.ValidateForRule()) {
                            alert("Lütfen " + item.Title + " parametre değerini giriniz");
                            result.push(0);
                            return false;
                        }
                    })
                }
                return result.length == 0;
            };
            var BindModel = (function (s) {
                _this.Parameters([]);
                if (s && s.Children) {
                    var i = 0;
                    s.Children.forEach(function (p) {
                        if (s.Parameters) {
                            p.Selected = ((typeof s.Parameters[i] == "string" ? s.Parameters[i].split(",") : s.Parameters[i]) || []).map(function (v) { return v.trim(); });
                            if (s.ParameterLabels && s.ParameterLabels[i])
                                p.ParameterLabels = s.ParameterLabels[i];

                            i++;
                        }
                        _this.Parameters.push(RuleUtility.Factory(p.Type, ParameterManager, p));
                    });

                }
                _this.MetadataId = ko.observable(s.Id);

            })(data);
        }
        return _super.NamedRuleStatement = NamedRuleStatement;
    })(StatementManager);
    return statementManager;
};
var RuleManager = function (ruleData) {
    var ruleManager = this;
    var Rule = (function (_super) {
        function Rule(data) {
            var _this = this;
            $.extend(_this, { TemplateName: 'RuleTreeStatement', Type: "", Parameters: [], Children: [], Id: 0 }, data);
            _this.Name = ko.observable(data.Name || "Kural");
            _this.Children = ko.observableArray([]);
            _this.IsActive = ko.observable(true);
            _this.Add = function () {
                if (_this.Children().length > 0)
                    _this.Children().forEach(function (i) {
                        i.Passive();
                    });
                _this.Children.push(RuleUtility.Factory("AndOrStatement", StatementManager, { Selected: "&&" }));
                _this.Children.push(RuleUtility.Factory("NamedRuleStatement", StatementManager, {}));
                ruleManager.BindPlugins({ statement: true });
            };
            _this.Remove = function (item) {
                if (_this.Children().length > 1) {
                    var index = _this.Children().indexOf(item);
                    index = index == 0 ? index + 1 : index - 1;
                    if (_this.Children()[index].GetTypeName() == "AndOrStatement")
                        _this.Children.remove(_this.Children()[index]);
                    _this.Children.remove(item);
                    ruleManager.BindPlugins({ statement: true });
                }
            };
            _this.Validate = function () {
                var result = [];
                if (_this.Name == "") {
                    alert("Lütfen kural adını giriniz");
                    result.push(0);
                }
                if (_this.Children && _this.Children().length > 0) {
                    _this.Children().forEach(function (item) {
                        if (!item.Validate()) {
                            result.push(0);
                            return false;
                        }
                    })
                }
                return result.length == 0;
            };
            _this.Normalize = function () {
                var willBeRemoved = [];
                ruleManager.TreeItems().forEach(function (rule) {
                    var children = rule.Children();
                    var andOrStatements = ko.utils.arrayFilter(children, function (item) {
                        return item.TemplateName === "AndOrStatement";
                    });
                    var normalized = [];
                    for (var i = 0; i < children.length; i++) {
                        var item = children[i];
                        if (item.TemplateName !== "AndOrStatement") {
                            if (i > 0 && children[i - 1].TemplateName !== "AndOrStatement") {
                                if (andOrStatements.length > 0)
                                    normalized.push(andOrStatements.shift());
                                else
                                    normalized.push(RuleUtility.Factory("AndOrStatement", StatementManager, { Selected: "&&" }));
                            }

                            normalized.push(item);
                            continue;
                        }

                        if (i > 0 && i < children.length - 1 && children[i - 1].TemplateName !== "AndOrStatement") {
                            if (andOrStatements.length > 0)
                                normalized.push(andOrStatements.shift());
                            else
                                normalized.push(RuleUtility.Factory("AndOrStatement", StatementManager, { Selected: "&&" }));
                        }
                    }
                    rule.Children(normalized);
                    if (normalized.length === 0)
                        willBeRemoved.push(rule);
                });
                for (var i = 0; i < willBeRemoved.length; i++) {
                    ruleManager.TreeItems.remove(willBeRemoved[i]);
                }
            };
            _this.OnMove = function () {
            };
            _this.UpdateSort = function () {
                _this.Normalize();
                ruleManager.BindPlugins({ statement: true })
            };
            _this.FocusStatement = function (item) {
                if (item) {
                    _this.Children().forEach(function (param) {
                        if (param !== item)
                            param.Passive()
                    })
                }
            };
            _this.Change = function (item) {
                if (item) {
                    item.Parameters([]);
                    var params = ruleManager.Metadatas().filter(function (m) { return m.Id === item.MetadataId() });
                    if (params && params.length > 0) {
                        item.Name = params[0].Name;
                        params[0].ParameterDefinations.forEach(function (param) {
                            if (param)
                                item.Parameters.push(RuleUtility.Factory(param.Type, ParameterManager, $.extend({ Selected: "" }, $.parseJSON(ko.toJSON(param)))))

                        });
                        ruleManager.BindPlugins({ statement: true });
                    }

                }
            };
            _this.Serialize = function () {
                var jsonData = $.parseJSON(ko.toJSON(_this));
                jsonData.Type = "RuleTreeStatement";
                jsonData.Children = [];
                $.each(_this.Children(), function (i, item) {
                    jsonData.Children.push(item.Serialize())
                });
                return jsonData;
            };
            _this.BindModel = (function (param) {
                _this.Children([]);
                if (param && param.Children) {
                    $.each(param.Children, function (i, item) {
                        if (RuleUtility.TypeIsDefine(item.Type, StatementManager)) {
                            if (item.Type === "NamedRuleStatement") {
                                var metadataItem = ruleManager.Metadatas().filter(function (m) { return m.Name === item.Name })[0];
                                if (metadataItem) { item.Children = metadataItem.ParameterDefinations; item.Id = metadataItem.Id; }
                            }
                            _this.Children.push(RuleUtility.Factory(item.Type, StatementManager, item));
                        }
                        else
                            console.error("tanımlı olmayan  statement bilgisi =>>" + item.Type)
                    });
                }
            })(data);
        }
        return _super.Rule = Rule;;
    })(RuleManager);
    ruleManager.TreeItems = ko.observableArray([]);
    ruleManager.Metadatas = ko.observableArray([]);
    ruleManager.Element = ruleData.Element || $("#RulePanel");
    ruleManager.Add = function () {
        var maxIndex = 0;
        ruleManager.TreeItems().filter(function (item) {
            return item.constructor === Rule
        }).forEach(function (tree) {
                var matched = tree.Name().match(/^(\d+)[.] Kural$/);
                if (matched && matched.length)
                    maxIndex = parseInt(matched[0]);
            
        });
        if (ruleManager.TreeItems().length > 0) {
            ruleManager.TreeItems.push(new StatementManager.AndOperatorStatement({ Selected: "||" }))
        }

        ruleManager.TreeItems.push(new Rule({
            Name: (maxIndex + 1) + ". Kural",
            Children: [RuleUtility.Factory("NamedRuleStatement", StatementManager, { Name: "FlightTest" })]
        }));
        ruleManager.BindPlugins();
    };
    ruleManager.Remove = function (item) {
        var index = ruleManager.TreeItems().indexOf(item);
        ruleManager.TreeItems.remove(item); 
        index = (index == 0 && ruleManager.TreeItems().length > 0) ? index  : index-1;
        if (ruleManager.TreeItems()[index] && ruleManager.TreeItems()[index].constructor === StatementManager.AndOrStatement) {
            ruleManager.TreeItems.remove(ruleManager.TreeItems()[index]);
        }
    };
    ruleManager.SetActive = function (item) {
        ruleManager.TreeItems().forEach(function (r) {(r && r.IsActive) ?r.IsActive(false):false; });
        (item && item.IsActive)? item.IsActive(true):false;
    };
    ruleManager.BindPlugins = function (options) {
        options = options || {};
        var container = ruleManager.Element;
        if (options["statement"]) {
            $(".list-parameter-select", container).select2({});
            $(".definition-parameter-select", container).select2({});
            $(".datetime-parameter", container).datetimepicker({
                format: 'DD.MM.YYYY',
                locale: 'tr',
                allowInputToggle: true,
                useCurrent: false,
                icons: {
                    time: "icon-schedule",
                    date: "icon-date_range",
                    up: "icon-keyboard_arrow_up",
                    down: "icon-keyboard_arrow_down",
                    previous: 'icon-keyboard_arrow_left',
                    next: 'icon-keyboard_arrow_right',
                    today: 'icon-signal_cellular_4_bar',
                    clear: 'icon-delete',
                    close: 'icon-close'
                }
            });
            if (typeof $.fn.destination !== 'undefined') {
                $(".destination-plugin", container).destination();
            }
            if (typeof $.fn.inputmask !== 'undefined') {
                $(".input-long-decimal-2").inputmask({ 'mask': '9.9999', rightAlign: false, allowPlus: false, allowMinus: false });
            }
        }
        if ($(".grd-form-group-lg").length > 0)
            $(".grd-form-group-lg").removeClass("grd-form-group-lg");
        $(".custom-select2.list-parameter-select,.custom-select2.metadataselect", container).each(function (i, obj) {
            if (!$(obj).data('select2')) {
                $(obj).select2();
            }
        });
    };
    ruleManager.Validate = function () {
        var result = [];

        if (ruleManager.TreeItems && ruleManager.TreeItems().length > 0) {
            ruleManager.TreeItems().forEach(function (item) {
                if (!item.Validate()) {
                    result.push(0);
                    return false;
                }
            })

        }
        return result.length == 0;
    };
    ruleManager.Serialize = function () {
        if (ruleManager.Validate()) {
            var data = {};
            data.TreeItems = ruleManager.TreeItems().map(function (i) { return i.Serialize(); });
            data.TreeItems.forEach(function (i) { delete i.ParameterLabels; });
            return data;
        }
        return false;
    };
    ruleManager.Save = function () {
        if (ruleManager.Validate()) {
            var data = ruleManager.Serialize();
            // Mock save function for demo
            console.log("Saving rule:", data);
            alert("Rule saved successfully!");
        }
    };
    ruleManager.BindModel = (function (data) {
        var sm = new StatementManager();
        var pm = new ParameterManager();
        if (data) {
            if (data.Metadatas) {
                if (Array.isArray(data.Metadatas)) {
                    // Array format (our project)
                    $.each(data.Metadatas, function (i, item) {
                        ruleManager.Metadatas.push($.extend({ Name: item.Name, Id: item.Id }, item))
                    });
                } else {
                    // Object format (original Gordios project)
                    $.each(Object.keys(data.Metadatas), function (i, item) {
                        ruleManager.Metadatas.push($.extend({ Name: item, Id: (i + 1) }, data.Metadatas[item]))
                    });
                }
            }
            if (data.TreeItems)
                $.each(data.TreeItems, function (i, item) {
                    if (item && item.Type != "IncorrectRuleStatement") {
                        if (item.Type == 'RuleTreeStatement') {
                            ruleManager.TreeItems.push(new Rule(item));
                        } else {
                            ruleManager.TreeItems.push(new StatementManager.AndOperatorStatement(item));
                        }
                    }
                })
        }
    })(ruleData);
    return ruleManager;
};
var ResultManager = function (resultData, isStatic) {
    var resultManager = this;
    resultManager.Metadatas = ko.observableArray([]);
    resultManager.MetadataId = ko.observable("");
    resultManager.BindPlugins = function (options) {
        options = $.extend({ isParameter: true }, options);
        var container = $("#RuleResultPanel");
        if (options["container"])
            container = $(options.container);
        if (!options.isParameter)
            $(".custom-select2").not(".array-parameter-select").select2();
        $("select", container).not(".custom-select2").select2();
        if (typeof $.fn.destination !== 'undefined') {
            $(".destination-plugin").destination();
        }
        if ($(".grd-form-group-lg").length > 0)
            $(".grd-form-group-lg").removeClass("grd-form-group-lg");
    };
    resultManager.TreeItems = ko.observableArray([]);
    resultManager.Change = function (item) {
        resultManager.TreeItems([]);
        var id = typeof item.MetadataId == "function" ? item.MetadataId() : item.MetadataId;
        var m = resultManager.Metadatas().filter(function (k) { return k.Id == id; });
        if (m) {
            var c = { Children: ko.observableArray([]) };
            if (m.length > 0) {
                m[0].Children = m[0].ParameterDefinations;
                c.Children.push(RuleUtility.Factory("NamedRuleStatement", StatementManager, m[0]));
                resultManager.TreeItems.push(c);
                resultManager.BindPlugins();
            }
        }
    };
    resultManager.Validate = function () {
        var r = [];
        if (resultManager.TreeItems()) {
            resultManager.TreeItems().forEach(function (item) {
                if (item.Children) {
                    item.Children().forEach(function (c) {
                        if (!c.Validate()) {
                            r.push(0);
                            return false;
                        }
                    })
                }
                if (r.length > 0)
                    return false;

            })
        }
        return r.length == 0;
    };
    resultManager.Serialize = function () {
        if (!resultManager.Validate())
            return false;
        var d = { TreeItems: [] };
        if (resultManager.TreeItems()) {
            resultManager.TreeItems().forEach(function (item) {
                var c = { Children: [], Type: "RuleTreeStatement" };
                if (item.Children) {
                    item.Children().forEach(function (i) {
                        c.Children.push(i.Serialize());
                    });
                    d.TreeItems.push(c);
                }
            })
        }
        return d;
    };
    var bindModel = (function (data) {
        var sm = new StatementManager();
        var pm = new ParameterManager();
        resultManager.TreeItems([]);
        if (data) {
            resultManager.Metadatas([]);
            if (data.Metadatas) {
                $.each(Object.keys(data.Metadatas), function (i, item) {
                    var metadataItem = $.extend({ Name: item, Id: (i + 1) }, data.Metadatas[item]);
                    if (data.Metadatas[item]) resultManager.Metadatas.push(metadataItem);

                })

            }
            var c = ko.observableArray([]);
            if (data.TreeItems) {
                data.TreeItems.forEach(function (t) {

                    if (t.Children) {
                        t.Children.forEach(function (s) {
                            if (s) {
                                var md = resultManager.Metadatas().filter(function (m) { return m.Name == s["Name"] })[0];
                                if (md) { s.Id = md.Id; s.Children = md.ParameterDefinations; resultManager.MetadataId = md.Id; s.Name = md.Name }
                                if (s.Type == "NamedRuleStatement")
                                    c.push(RuleUtility.Factory(s.Type, StatementManager, s));
                            }
                        });
                    }

                })
            }
            if (isStatic) {
                resultManager.Metadatas().forEach(function (m) {
                    var mi = c().filter(function (i) {
                        var mid = typeof i["MetadataId"] == "function" ? i.MetadataId() : i.MetadataId;
                        return m.Id == mid;
                    })[0];
                    if (!mi) {
                        c.push(RuleUtility.Factory("NamedRuleStatement", StatementManager, { Id: m.Id, Children: m.ParameterDefinations, Name: m.Title }));
                    }
                })
            }
            if (c().length > 0)
                resultManager.TreeItems.push({ Type: "RuleTreeStatement", Children: c })
        }
    })(resultData);
};

var rule = {
    metadata: {
        init: function (data) {
            console.log("Metadata init called with data:", data);
            $(document).ready(function () {
                console.log("Document ready, creating MetadataManager...");
                var model = { MetadataModel: new MetadataManager(data || {}) };
                console.log("MetadataManager created:", model);
                console.log("CategoryItems:", model.MetadataModel.CategoryItems());
                console.log("ParameterTypes:", model.MetadataModel.ParameterTypes());
                
                ko.applyBindings(model);
                console.log("Knockout bindings applied");
                
                model.MetadataModel.BindPlugins({ select: true, editor: true });
                console.log("Plugins bound");
                
                if (typeof $.fn.select2 !== 'undefined') {
                    $(".category").select2({ tags: true, placeholder: "Seçiniz" });
                    console.log("Select2 initialized for categories");
                } else {
                    console.error("Select2 not available!");
                }
            })
        }
    },
    ruleManager: {
        init: function (model, predicatePanel, resultPanel) {
            console.log("RuleManager.init called with model:", model);
            
            if (predicatePanel) {
                if (typeof (predicatePanel) == "string")
                    predicatePanel = $(predicatePanel)
            } else
                predicatePanel = $("#RulePanel")[0];

            if (resultPanel) {
                if (typeof (resultPanel) == "string")
                    resultPanel = $(resultPanel)
            } else
                resultPanel = $("#RuleResultPanel")[0];
                
            console.log("PredicatePanel:", predicatePanel);
            console.log("ResultPanel:", resultPanel);


            var rules = null, ruleResult = null;

            if (model.RuleItem) {
                var k = model.RuleItem || {};
                k.Element = predicatePanel;
                /* Rule item bind*/
                rules = { RuleModel: new RuleManager(k) };
                ko.applyBindings(rules, predicatePanel);
                rules.RuleModel.BindPlugins();
                if (isStaticRule) {
                    var r = { StaticRuleModel: new ResultManager(model.RuleItem || {}, true) };
                    ko.applyBindings(r, document.getElementById("StaticDesign"));
                    r.StaticRuleModel.BindPlugins({ isParameter: true, container: "#StaticDesign" });
                }
            } else {
                // Fallback: create empty RuleManager if RuleItem is not provided
                var k = { Name: "Sample Rule", Children: [], Metadatas: [] };
                k.Element = predicatePanel;
                rules = { RuleModel: new RuleManager(k) };
                ko.applyBindings(rules, predicatePanel);
                rules.RuleModel.BindPlugins();
            }
            if (model.ResultItem && resultPanel) {
                /*Result item bind*/
                ruleResult = { ResultModel: new ResultManager(model.ResultItem || {}) };
                ko.applyBindings(ruleResult, resultPanel);
                ruleResult.ResultModel.BindPlugins({ isParameter: false });
            }
            console.log("Returning rules:", rules);
            console.log("Returning ruleResult:", ruleResult);
            
            var result = {
                Rules: rules ? rules["RuleModel"] : null,
                Result: ruleResult ? ruleResult["ResultModel"] : null
            };
            
            console.log("Final result:", result);
            return result;

        }
    }
};