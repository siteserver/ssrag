﻿var $url = "/cms/contents/contentsLayerImport";

var data = utils.init({
  checkedLevels: null,
  form: {
    siteId: utils.getQueryInt("siteId"),
    channelId: utils.getQueryInt("channelId"),
    importType: "zip",
    checkedLevel: null,
    isOverride: false,
    fileNames: [],
    fileUrls: [],
    attributes: [],
  },
  excelExcludes: [
    "序号",
    "内容Id",
    "识别码",
    "最后修改时间",
    "内容组",
    "标签",
    "添加人",
    "最后修改人",
    "投稿用户",
    "来源标识",
    "内容模板",
    "点击量",
    "下载量",
    "审核人",
    "审核时间",
    "审核意见",
  ],
  uploadUrl: null,
  uploadList: [],
  orderedFileNames: [],
  columns: [],
  styles: [],
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url, {
        params: {
          siteId: this.form.siteId,
          channelId: this.form.channelId,
        },
      })
      .then(function (response) {
        var res = response.data;

        $this.checkedLevels = res.checkedLevels;
        $this.form.checkedLevel = res.value;
        $this.form.importType = res.options.importType;
        $this.form.isOverride = res.options.isOverride;

        $this.btnRadioInput();
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSubmit: function () {
    var $this = this;

    var fileNames = [];
    var fileUrls = [];
    for (var i = 0; i < this.orderedFileNames.length; i++) {
      var fileName = this.orderedFileNames[i];
      var index = this.form.fileNames.indexOf(fileName);
      if (index !== -1) {
        fileNames.push(this.form.fileNames[index]);
        fileUrls.push(this.form.fileUrls[index]);
      }
    }
    for (var i = 0; i < this.form.fileNames.length; i++) {
      var fileName = this.form.fileNames[i];
      var index = this.orderedFileNames.indexOf(fileName);
      if (index === -1) {
        fileNames.push(this.form.fileNames[i]);
        fileUrls.push(this.form.fileUrls[i]);
      }
    }
    fileNames.reverse();
    fileUrls.reverse();

    utils.loading(this, true);
    $api
      .post($url, {
        siteId: this.form.siteId,
        channelId: this.form.channelId,
        importType: this.form.importType,
        checkedLevel: this.form.checkedLevel,
        isOverride: this.form.isOverride,
        fileNames: fileNames,
        fileUrls: fileUrls,
        attributes: this.form.attributes,
      })
      .then(function (response) {
        var res = response.data;

        utils.closeLayer();
        parent.$vue.apiList($this.form.channelId, 1, "文件导入成功！", true);
        if (res.isIndex) {
          for (var i = 0; i < res.contentIds.length; i++) {
            parent.$vue.runAiTaskIndex($this.form.channelId, res.contentIds[i]);
          }
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnRadioInput: function () {
    this.uploadUrl =
      $apiUrl +
      $url +
      "/actions/upload?siteId=" +
      this.form.siteId +
      "&channelId=" +
      this.form.channelId +
      "&importType=" +
      this.form.importType;
  },

  btnSubmitClick: function () {
    if (this.form.fileNames.length === 0) {
      return utils.error("请选择需要导入的文件！");
    }

    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadBefore(file) {
    var re = /(\.zip|\.xlsx|\.txt)$/i;
    if (this.form.importType === "zip") {
      re = /(\.zip)$/i;
    } else if (this.form.importType === "xlsx") {
      re = /(\.xlsx)$/i;
    } else if (this.form.importType === "image") {
      re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    } else if (this.form.importType === "txt") {
      re = /(\.txt)$/i;
    }
    if (!re.exec(file.name)) {
      utils.error("请选择有效的文件上传!");
      return false;
    }
    this.orderedFileNames.push(file.name);
    return true;
  },

  uploadProgress: function () {
    utils.loading(this, true);
  },

  uploadRemove(file) {
    if (file.response) {
      var index = this.form.fileNames.indexOf(file.response.name);
      this.form.fileNames.splice(index, 1);
      this.form.fileUrls.splice(index, 1);

      var selectedIndex = this.orderedFileNames.indexOf(file.response.name);
      this.orderedFileNames.splice(selectedIndex, 1);
    }
  },

  uploadSuccess: function (res) {
    if (this.form.importType === "excel") {
      this.form.fileNames = [];
      this.form.fileUrls = [];
      this.columns = res.columns;
      this.styles = res.styles;
      this.form.attributes = [];
      for (var i = 0; i < this.columns.length; i++) {
        this.form.attributes[i] = "";

        var column = this.columns[i];
        for (var j = 0; j < this.styles.length; j++) {
          var style = this.styles[j];
          if (style.attributeName == column || style.displayName == column) {
            this.form.attributes[i] = style.attributeName;
          }
        }
      }
    }
    this.form.fileNames.push(res.name);
    this.form.fileUrls.push(res.url);
    utils.loading(this, false);
  },

  uploadError: function (err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  },
});
