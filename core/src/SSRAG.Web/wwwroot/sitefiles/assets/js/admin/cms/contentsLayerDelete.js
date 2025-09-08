﻿var $url = "/cms/contents/contentsLayerDelete";

var data = utils.init({
  page: utils.getQueryInt("page"),
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  fileName: utils.getQueryString("fileName"),
  contents: null,
  form: {
    isRetainFiles: false,
  },
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url, {
        params: {
          siteId: this.siteId,
          channelId: this.channelId,
          fileName: this.fileName,
        },
      })
      .then(function (response) {
        var res = response.data;

        $this.contents = res.contents;
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

    utils.loading(this, true);
    $api
      .post($url, {
        siteId: this.siteId,
        channelId: this.channelId,
        fileName: this.fileName,
        isRetainFiles: this.form.isRetainFiles,
      })
      .then(function (response) {
        var res = response.data;

        parent.$vue.apiList($this.channelId, $this.page, "内容删除成功!", true);
        for (var i = 0; i < $this.contents.length; i++) {
          parent.$vue.runAiTaskRemove($this.channelId, $this.contents[i].id);
        }
        utils.closeLayer();
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return utils.getRootUrl("redirect", {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id,
      });
    }
    return $apiUrl + "/preview/" + content.siteId + "/" + content.channelId + "/" + content.id;
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
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
