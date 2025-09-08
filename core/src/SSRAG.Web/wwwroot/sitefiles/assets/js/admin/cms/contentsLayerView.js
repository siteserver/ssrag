﻿var $url = "/cms/contents/contentsLayerView";
var $urlPreview = $url + "/actions/preview";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  contentId: utils.getQueryInt("contentId"),
  content: null,
  channelName: null,
  state: null,
  columns: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  editorColumns: null,
  isCheckable: false,
  activeName: "titleBody",
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
          contentId: this.contentId,
        },
      })
      .then(function (response) {
        var res = response.data;

        $this.content = res.content;
        $this.channelName = res.channelName;
        ($this.state = res.state), ($this.columns = res.columns);
        $this.siteUrl = res.siteUrl;
        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.editorColumns = res.editorColumns;
        $this.isCheckable = !res.content.checked && res.isCheckable;
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
      .post($urlPreview, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
      })
      .then(function (response) {
        var res = response.data;

        window.open(res.url);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  isDisplay: function (attributeName) {
    return (
      attributeName !== "Sequence" && attributeName !== "Title" && attributeName !== "ChannelId"
    );
  },

  getLinkType: function (linkType) {
    if (linkType === "LinkToFirstChannel") {
      return "链接到第一个子栏目";
    } else if (linkType === "LinkToChannel") {
      return "链接到指定栏目";
    } else if (linkType === "LinkToContent") {
      return "链接到指定内容";
    } else if (linkType === "LinkToFirstContent") {
      return "链接到第一条内容";
    } else if (linkType === "LinkToOnlyOneContent") {
      return "仅一条内容时链接到此内容";
    } else if (linkType === "NoLink") {
      return "不可链接";
    }
    return "默认";
  },

  getUrl: function (virtualUrl) {
    if (!virtualUrl) return "";
    return _.replace(virtualUrl, "@/", this.siteUrl + "/");
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

  btnAdminClick: function (uuid) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getCommonUrl("adminLayerView", { uuid: uuid }),
      full: true,
    });
  },

  btnUserClick: function (uuid) {
    utils.openLayer({
      title: "用户查看",
      url: utils.getCommonUrl("userLayerView", { uuid: uuid }),
      full: true,
    });
  },

  btnCheckClick: function () {
    window.parent.layer.closeAll();
    window.parent.utils.openLayer({
      title: "审核内容",
      url: utils.getCmsUrl("contentsLayerCheck", {
        siteId: this.siteId,
        channelId: this.channelId,
        channelContentIds: this.channelId + "_" + this.contentId,
      }),
      full: true,
    });
  },

  btnPreviewClick: function () {
    this.apiPreview();
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
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  },
});
