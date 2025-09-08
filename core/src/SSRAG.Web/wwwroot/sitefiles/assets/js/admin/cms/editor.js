var $url = "/cms/editor";
var $urlInsert = $url + "/actions/insert";
var $urlUpdate = $url + "/actions/update";
var $urlUpload = $url + "/actions/upload";
var $urlPreview = $url + "/actions/preview";
var $urlTags = $url + "/actions/tags";

Date.prototype.Format = function (fmt) {
  var o = {
    "M+": this.getMonth() + 1, // 月份
    "d+": this.getDate(), // 日
    "h+": this.getHours(), // 时
    "m+": this.getMinutes(), // 分
    "s+": this.getSeconds(), // 秒
    "q+": Math.floor((this.getMonth() + 3) / 3), // 季度
    S: this.getMilliseconds(), // 毫秒
  };

  if (/(y+)/.test(fmt))
    fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

  for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt))
      fmt = fmt.replace(
        RegExp.$1,
        RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length)
      );

  return fmt;
};

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  contentId: utils.getQueryInt("contentId"),
  page: utils.getQueryInt("page"),
  tabName: utils.getQueryString("tabName"),
  reloadChannelId: utils.getQueryInt("reloadChannelId"),
  mainHeight: "",
  isSettings: true,
  sideType: "settings",
  collapseSettings: ["checkedLevel", "addDate"],
  collapseMore: ["templateId", "translates"],

  csrfToken: null,
  site: null,
  siteUrl: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: null,
  linkTypes: null,
  linkTo: {
    channelIds: [],
    contentId: 0,
    contentTitle: "",
  },
  root: null,
  styles: null,
  relatedFields: null,
  templates: null,
  form: null,
  breadcrumbItems: [],
  translates: [],
  isPreviewSaving: false,
  isScheduledDialog: false,
  scheduledForm: {
    isScheduled: false,
    scheduledDate: new Date(),
  },
  settings: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    window.onresize = function () {
      $this.mainHeight = $(window).height() - 70 + "px";
    };
    window.onresize();

    $api
      .get($url, {
        params: {
          siteId: $this.siteId,
          channelId: $this.channelId,
          contentId: $this.contentId,
        },
      })
      .then(function (response) {
        var res = response.data;
        if (res.channel.isChangeBanned) {
          return utils.alertWarning({
            title: "禁止修改内容",
            text: "栏目已开启禁止维护内容(添加/修改/删除)功能，修改内容请先在栏目中关闭此功能！",
            callback: function () {
              utils.removeTab();
            },
          });
        }

        $this.csrfToken = res.csrfToken;

        $this.site = res.site;
        $this.siteUrl = res.siteUrl;
        $this.channel = res.channel;

        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.checkedLevels = res.checkedLevels;
        $this.linkTypes = res.linkTypes;
        $this.linkTo = res.linkTo;
        $this.root = [res.root];
        $this.settings = res.settings;

        $this.styles = res.styles;
        $this.relatedFields = res.relatedFields;
        $this.templates = res.templates;
        $this.form = _.assign({}, res.content);
        $this.breadcrumbItems = res.breadcrumbItems;

        $this.scheduledForm.isScheduled = res.isScheduled;
        $this.scheduledForm.scheduledDate = res.scheduledDate;

        if (!$this.form.addDate) {
          $this.form.addDate = new Date().Format("yyyy-MM-dd hh:mm:ss");
        } else {
          $this.form.addDate = new Date($this.form.addDate).Format("yyyy-MM-dd hh:mm:ss");
        }

        if ($this.form.checked) {
          $this.form.checkedLevel = $this.site.checkContentLevel;
        }
        var targetCheckedLevel = $this.checkedLevels.find(
          (x) => x.value === $this.form.checkedLevel
        );
        if (!!!targetCheckedLevel) {
          $this.form.checkedLevel = res.checkedLevel;
        }
        if ($this.form.top || $this.form.recommend || $this.form.hot || $this.form.color) {
          $this.collapseSettings.push("attributes");
        }
        if ($this.form.groupNames && $this.form.groupNames.length > 0) {
          $this.collapseSettings.push("groupNames");
        } else {
          $this.form.groupNames = [];
        }
        if ($this.form.tagNames && $this.form.tagNames.length > 0) {
          $this.collapseSettings.push("tagNames");
        } else {
          $this.form.tagNames = [];
        }
        if (($this.form.linkType && $this.form.linkType != "None") || $this.form.linkUrl) {
          $this.collapseSettings.push("link");
        }

        for (var i = 0; i < $this.styles.length; i++) {
          var style = $this.styles[i];
          if (style.inputType === "CheckBox" || style.inputType === "SelectMultiple") {
            var value = $this.form[utils.toCamelCase(style.attributeName)];
            if (!Array.isArray(value)) {
              if (!value) {
                $this.form[utils.toCamelCase(style.attributeName)] = [];
              } else {
                $this.form[utils.toCamelCase(style.attributeName)] = utils.toArray(value);
              }
            }
          } else if (
            style.inputType === "Image" ||
            style.inputType === "File" ||
            style.inputType === "Video"
          ) {
            $this.form[utils.getCountName(style.attributeName)] = utils.toInt(
              $this.form[utils.getCountName(style.attributeName)]
            );
          } else if (
            style.inputType === "Text" ||
            style.inputType === "TextArea" ||
            style.inputType === "TextEditor"
          ) {
            if ($this.contentId === 0) {
              $this.form[utils.toCamelCase(style.attributeName)] = style.defaultValue;
            }
          }
        }

        setTimeout(function () {
          for (var i = 0; i < $this.styles.length; i++) {
            var style = $this.styles[i];
            if (style.inputType === "TextEditor") {
              var editor = utils.getEditor(style.attributeName, style.height);
              editor.styleIndex = i;
              editor.ready(function () {
                this.addListener("contentChange", function () {
                  var style = $this.styles[this.styleIndex];
                  $this.form[utils.toCamelCase(style.attributeName)] = this.getContent();
                });
              });
            }
          }
        }, 100);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiInsert: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlInsert, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
        translates: this.translates,
        linkTo: this.linkTo,
        isScheduled: this.scheduledForm.isScheduled,
        scheduledDate: this.scheduledForm.scheduledDate,
      })
      .then(function (response) {
        var res = response.data;

        $this.closeAndRedirect(false, res.contentId, res.isIndex, res.isRemove);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  getChannelUrl: function (data) {
    return utils.getRootUrl("redirect", {
      siteId: this.siteId,
      channelId: data.value,
    });
  },

  getText: function () {
    var text = "";
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.inputType === "TextEditor") {
        var editor = utils.getEditor(style.attributeName);
        text += editor.getContent();
      }
    }
    var replaceWords = [];
    for (var word of replaceWords) {
      text = text.replace(new RegExp(word, "g"), "");
    }
    return text;
  },

  apiTags: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlTags, {
        siteId: this.siteId,
        channelId: this.channelId,
        content: this.form.body,
      })
      .then(function (response) {
        var res = response.data;

        if (res.tags && res.tags.length > 0) {
          $this.form.tagNames = _.union($this.form.tagNames, res.tags);
          utils.success("成功提取标签！");
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiPreview: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlPreview, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
      })
      .then(function (response) {
        var res = response.data;

        $this.isPreviewSaving = false;
        window.open(res.url);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiUpdate: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlUpdate, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
        translates: this.translates,
        linkTo: this.linkTo,
        isScheduled: this.scheduledForm.isScheduled,
        scheduledDate: this.scheduledForm.scheduledDate,
      })
      .then(function (response) {
        var res = response.data;

        $this.closeAndRedirect(true, res.contentId, res.isIndex, res.isRemove);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiUpload: function (type, file) {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlUpload + "?siteId=" + this.siteId + "&type=" + type, file)
      .then(function (response) {
        var res = response.data;
        $this.insertLatestText(type, res.value);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSave: function () {
    if (this.contentId === 0) {
      this.apiInsert();
    } else {
      this.apiUpdate();
    }
  },

  runFormLayerImageUploadText: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerImageUploadEditor: function (attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runFormLayerFileUpload: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerVideoUpload: function (attributeName, no, text, coverUrl) {
    this.insertText(attributeName, no, text);
    if (coverUrl) {
      this.runFormLayerImageUploadText("ImageUrl", no, coverUrl);
    }
  },

  runEditorLayerImage: function (attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runLayerContentSelect: function (content) {
    this.linkTo.contentId = content.id;
    this.linkTo.contentTitle = content.title;
  },

  getContentUrl: function () {
    return utils.getRootUrl("redirect", {
      siteId: this.siteId,
      channelId: this.linkTo.channelIds[this.linkTo.channelIds.length - 1],
      contentId: this.linkTo.contentId,
    });
  },

  insertText: function (attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    if (count <= no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
    utils.focusTab();
  },

  insertLatestText: function (attributeName, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    var value = this.form[utils.getExtendName(attributeName, count)];
    if (value) {
      this.insertText(attributeName, count + 1, text);
    } else {
      this.insertText(attributeName, count, text);
    }
  },

  insertEditor: function (attributeName, html) {
    if (!attributeName) attributeName = "Body";
    if (!html) return;
    utils.getEditor(attributeName).execCommand("insertHTML", html);
    utils.focusTab();
  },

  addTranslation: function (targetSiteId, targetChannelId, translateType, summary) {
    this.translates.push({
      siteId: this.siteId,
      channelId: this.channelId,
      targetSiteId: targetSiteId,
      targetChannelId: targetChannelId,
      translateType: translateType,
      summary: summary,
    });
  },

  updateGroups: function (res, message) {
    this.groupNames = res.groupNames;
    utils.success(message);
  },

  syncEditors: function () {
    var $this = this;
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
        var style = $this.styles[editor.styleIndex];
        var text = editor.getContent();
        $this.form[utils.toCamelCase(style.attributeName)] = text;
      });
    }
  },

  regexReplace: function (regex, text) {
    var retVal = text;
    while ((match = regex.exec(text)) !== null) {
      retVal = retVal.replace(match[0], match[1]);
    }
    return retVal;
  },

  closeAndRedirect: function (isEdit, contentId, isIndex, isRemove) {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      if (isEdit) {
        tabVue.apiList(
          this.reloadChannelId > 0 ? this.reloadChannelId : this.channelId,
          this.page,
          "内容编辑成功！"
        );
      } else {
        tabVue.apiList(this.channelId, this.page, "内容新增成功！", true);
      }
      if (isIndex) {
        tabVue.runAiTaskIndex(this.channelId, contentId);
      }
      if (isRemove) {
        tabVue.runAiTaskRemove(this.channelId, contentId);
      }
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  btnImageSelectClick: function (args) {
    var inputType = args.inputType;
    var attributeName = args.attributeName;
    var no = args.no;
    var type = args.type;

    if (type === "uploadedImages") {
      this.btnLayerClick({
        title: "选择已上传图片",
        name: "formLayerImageSelect",
        inputType: inputType,
        attributeName: attributeName,
        no: no,
        full: true,
      });
    }
  },

  btnLayerClick: function (options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId,
      editorAttributeName: "Body",
    };

    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.inputType) {
      query.inputType = options.inputType;
    }
    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query),
    };
    if (!options.full) {
      args.width = options.width ? options.width : 750;
      args.height = options.height ? options.height : 550;
    }

    utils.openLayer(args);
  },

  handleTranslationClose: function (summary) {
    this.translates = _.remove(this.translates, function (n) {
      return summary !== n.summary;
    });
  },

  btnSaveCommandClick: function (command) {
    this.isScheduledDialog = command === "scheduled";
  },

  btnSubmitClick: function () {
    var $this = this;
    this.syncEditors();
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.$refs.linkToForm.validate(function (valid) {
          if (valid) {
            $this.apiSave();
          }
        });
      } else {
        utils.scrollToError();
      }
    });
  },

  btnScheduledSaveClick: function () {
    var $this = this;
    this.$refs.scheduledForm.validate(function (valid) {
      if (valid) {
        var minutesDate = new Date();
        minutesDate.setMinutes(minutesDate.getMinutes() + 5);
        if (
          !$this.scheduledForm.scheduledDate ||
          new Date($this.scheduledForm.scheduledDate).getTime() < minutesDate.getTime()
        ) {
          utils.error("定时发布失败，定时发布时间只能是5分钟之后的某一时刻");
          return;
        }

        $this.isScheduledDialog = false;
        $this.btnSubmitClick();
      }
    });
  },

  btnSaveClick: function () {
    this.scheduledForm.isScheduled = false;
    this.btnSubmitClick();
  },

  btnTagsClick: function () {
    this.syncEditors();
    if (!this.form.body) return;
    this.apiTags();
  },

  btnPreviewClick: function () {
    var $this = this;
    if (this.isPreviewSaving) return;
    this.syncEditors();
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.apiPreview();
      } else {
        utils.scrollToError();
      }
    });
  },

  btnCloseClick: function () {
    utils.removeTab();
  },

  btnGroupAddClick: function () {
    utils.openLayer({
      title: "新增内容组",
      url: utils.getCommonUrl("groupContentLayerAdd", { siteId: this.siteId }),
      width: 500,
      height: 300,
    });
  },

  btnTranslateAddClick: function () {
    utils.openLayer({
      title: "选择转移栏目",
      url: utils.getCmsUrl("editorLayerTranslate", {
        siteId: this.siteId,
        channelId: this.channelId,
      }),
      width: 620,
      height: 400,
    });
  },

  btnLinkToContentClick: function () {
    var channelId = this.linkTo.channelIds[this.linkTo.channelIds.length - 1];
    utils.openLayer({
      title: "选择指定内容",
      url: utils.getCmsUrl("layerContentSelect", {
        siteId: this.siteId,
        channelId: channelId,
        contentId: this.contentId,
      }),
    });
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: _.assign({}, methods, _partialForm),
  created: function () {
    utils.keyPress(this.btnSaveClick, this.btnCloseClick);
    this.apiGet();
    var $this = this;
    document.onpaste = function (event) {
      var items = (event.clipboardData || event.originalEvent.clipboardData).items;
      for (var i = items.length; i > 0; i--) {
        var item = items[i - 1];
        if (item.kind !== "file") continue;
        var type = item.type;
        if (!type || type.indexOf("/") === -1) continue;
        if (type === "application/vnd.openxmlformats-officedocument.wordprocessingml.document") {
          type = "file.docx";
        } else if (type === "application/msword") {
          type = "file.doc";
        }

        if (type.indexOf("image/") !== -1) {
          var blob = item.getAsFile();
          var formData = new FormData();
          formData.append("file", blob, type.replace("/", "."));
          $this.apiUpload("ImageUrl", formData);
        } else if (type.indexOf("video/") !== -1) {
          var blob = item.getAsFile();
          var formData = new FormData();
          formData.append("file", blob, type.replace("/", "."));
          $this.apiUpload("VideoUrl", formData);
        } else {
          var blob = item.getAsFile();
          var formData = new FormData();
          formData.append("file", blob, type.replace("/", "."));
          $this.apiUpload("FileUrl", formData);
        }
      }
    };
  },
});
