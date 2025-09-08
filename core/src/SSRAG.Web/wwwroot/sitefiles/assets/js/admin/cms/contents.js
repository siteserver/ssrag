﻿var $url = "/cms/contents/contents";
var $urlTree = $url + "/actions/tree";
var $urlList = $url + "/actions/list";
var $urlColumns = $url + "/actions/columns";
var $urlWidth = $url + "/actions/width";
var $urlOrder = $url + "/actions/order";
var $urlSaveIds = $url + "/actions/saveIds";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId") || utils.getQueryInt("siteId"),
  root: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: [],
  expendedChannelIds: [],

  filterText: "",

  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  titleColumn: null,
  columns: null,
  permissions: null,
  contentMenus: null,
  contentsMenus: null,
  asideHeight: 0,
  tableMaxHeight: 0,
  multipleSelection: [],
  checkedColumns: [],
  breadcrumbItems: [],
  isChangeBanned: false,
  isKnowledge: false,

  searchForm: {
    searchType: "Title",
    searchText: "",
    isAllContents: false,
  },

  isAdvancedForm: false,
  advancedForm: {
    checkedLevels: [],
    isTop: false,
    isRecommend: false,
    isHot: false,
    isColor: false,
    isIndex: false,
    groupNames: [],
    tagNames: [],
  },
});

var methods = {
  runAiTaskIndex: function (channelId, contentId) {
    $aiApi.tasksIndex(this.siteId, channelId, contentId);
  },

  runAiTaskRemove: function (channelId, contentId) {
    $aiApi.tasksRemove(this.siteId, channelId, contentId);
  },

  apiTree: function (reload) {
    var $this = this;

    $api
      .post($urlTree, {
        siteId: this.siteId,
        reload: reload,
      })
      .then(function (response) {
        var res = response.data;

        $this.root = [res.root];
        if (!reload) {
          utils.loadExternals(res.cssUrls, res.jsUrls);
          $this.siteUrl = res.siteUrl;
          $this.groupNames = res.groupNames;
          $this.tagNames = res.tagNames;
          $this.checkedLevels = res.checkedLevels;
          $this.advancedForm.checkedLevels = _.map(res.checkedLevels, function (x) {
            return x.label;
          });
          $this.expendedChannelIds = [$this.siteId];
        } else {
          $this.expendedChannelIds = [$this.siteId, $this.channelId];
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        $this.asideHeight = $(window).height() - 4;
        $this.tableMaxHeight = $(window).height() - 128;
        utils.loading($this, false);
      });
  },

  apiList: function (channelId, page, message, reload) {
    var $this = this;

    utils.loading(this, true);
    var request = _.assign(
      {
        siteId: this.siteId,
        channelId: channelId,
        page: page,
        searchType: this.searchForm.searchType,
        searchText: this.searchForm.searchText,
        isAdvanced: this.isAdvanced,
      },
      this.advancedForm
    );
    $api
      .post($urlList, request)
      .then(function (response) {
        var res = response.data;

        $this.pageContents = res.pageContents;
        $this.titleColumn = res.titleColumn;
        $this.columns = res.columns;
        $this.total = res.total;
        $this.pageSize = res.pageSize;
        $this.page = page;
        $this.permissions = res.permissions;
        $this.contentMenus = res.contentMenus;
        $this.contentsMenus = res.contentsMenus;
        $this.expendedChannelIds = [$this.siteId, channelId];
        $this.searchForm.isAllContents = res.isAllContents;
        $this.isChangeBanned = res.isChangeBanned;
        $this.isKnowledge = res.isKnowledge;
        $this.breadcrumbItems = res.breadcrumbItems;

        if (message) {
          utils.success(message);
        }
        if (reload) {
          $this.apiTree(true);
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
        $this.scrollToTop();
      });
  },

  apiColumns: function (attributeNames) {
    $api
      .post($urlColumns, {
        siteId: this.siteId,
        channelId: this.channelId,
        attributeNames: attributeNames,
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  apiWidth: function (prevAttributeName, prevWidth, nextAttributeName, nextWidth) {
    $api
      .post($urlWidth, {
        siteId: this.siteId,
        channelId: this.channelId,
        prevAttributeName: prevAttributeName || "",
        prevWidth: parseInt(prevWidth || 0),
        nextAttributeName: nextAttributeName || "",
        nextWidth: parseInt(nextWidth || 0),
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  apiOrder: function (channelId, contentId, isUp, rows) {
    var $this = this;

    utils.loading(this, true);
    $api
      .post($urlOrder, {
        siteId: this.siteId,
        channelId: channelId,
        contentId: contentId,
        isUp: isUp,
        rows: rows,
      })
      .then(function (response) {
        var res = response.data;

        $this.apiList($this.channelId, $this.page, "内容排序成功！");
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSaveIds: function (callback) {
    var $this = this;

    utils.loading(this, true);
    $api
      .post($urlSaveIds, {
        siteId: this.siteId,
        channelContentIds: this.channelContentIds,
      })
      .then(function (response) {
        var res = response.data;

        callback(res.value);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  handleAllChange: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .post($url + "/actions/all", {
        siteId: this.siteId,
        channelId: this.channelId,
        isAllContents: this.searchForm.isAllContents,
      })
      .then(function (response) {
        var res = response.data;

        $this.apiList($this.channelId, 1);
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  getChannelUrl: function (data) {
    return utils.getRootUrl("redirect", {
      siteId: this.siteId,
      channelId: data.value,
    });
  },

  getContentUrl: function (content) {
    if (content.linkType == "NoLink") {
      return "javascript:;";
    }
    if (content.referenceId > 0 && content.sourceId > 0) {
      return utils.getRootUrl("redirect", {
        siteId: content.siteId,
        channelId: content.sourceId,
        contentId: content.referenceId,
      });
    }
    return utils.getRootUrl("redirect", {
      siteId: content.siteId,
      channelId: content.channelId,
      contentId: content.id,
    });
  },

  getContentTarget: function (content) {
    if (content.linkType == "NoLink") {
      return "";
    }
    return "_blank";
  },

  btnTitleClick: function (content) {
    if (content.checked && content.channelId > 0) return false;
    utils.openLayer({
      title: "查看内容",
      url: utils.getCmsUrl("contentsLayerView", {
        siteId: this.siteId,
        channelId: Math.abs(content.channelId),
        contentId: content.id,
      }),
      full: true,
    });
  },

  btnSearchClick: function () {
    this.isAdvancedForm = false;
    this.apiList(this.channelId, 1);
  },

  handleTagNamesChange: function (visible) {
    if (!visible) {
      this.isAdvancedForm = false;
      this.apiList(this.channelId, 1);
    }
  },

  btnAIWriterClick: function () {
    if (this.isChangeBanned) {
      utils.alertWarning({
        title: "禁止添加内容",
        text: "栏目已开启禁止维护内容(添加/修改/删除)功能，添加内容请先在栏目中关闭此功能！",
      });
    } else {
      utils.addTab(
        "AI撰写",
        utils.getDatasetUrl("writer", {
          siteId: this.siteId,
          channelId: this.channelId,
          page: this.page,
          tabName: utils.getTabName(),
        })
      );
    }
  },

  btnAddClick: function () {
    if (this.isChangeBanned) {
      utils.alertWarning({
        title: "禁止添加内容",
        text: "栏目已开启禁止维护内容(添加/修改/删除)功能，添加内容请先在栏目中关闭此功能！",
      });
    } else {
      utils.addTab("添加内容", this.getAddUrl());
    }
  },

  btnImportClick: function (command) {
    if (command === "Word") {
      this.btnLayerClick({ title: "批量导入Word", name: "Word", full: true });
    } else if (command === "Import") {
      this.btnLayerClick({ title: "批量导入", name: "Import", full: true });
    } else if (command === "Add") {
      this.btnLayerClick({ title: "批量添加", name: "Add", full: true });
    }
  },

  btnMoreClick: function (command) {
    if (command === "Group") {
      this.btnLayerClick({
        title: "批量设置分组",
        name: "Group",
        width: 700,
        height: 400,
        saveIds: true,
      });
    } else if (command === "Tag") {
      this.btnLayerClick({
        title: "批量设置标签",
        name: "Tag",
        width: 700,
        height: 400,
        saveIds: true,
      });
    } else if (command === "ExportAll") {
      this.btnLayerClick({ title: "导出全部", name: "Export", full: true });
    } else if (command === "ExportSelected") {
      this.btnLayerClick({ title: "导出选中", name: "Export", full: true, saveIds: true });
    } else if (command === "Arrange") {
      this.btnLayerClick({ title: "整理排序", name: "Arrange", width: 550, height: 350 });
    } else if (command === "Hits") {
      this.btnLayerClick({
        title: "设置点击量",
        name: "Hits",
        width: 450,
        height: 320,
        saveIds: true,
      });
    }
  },

  btnEditClick: function (content) {
    if (!this.permissions.isEdit) return;
    if (this.isChangeBanned) {
      utils.alertWarning({
        title: "禁止修改内容",
        text: "栏目已开启禁止维护内容(添加/修改/删除)功能，修改内容请先在栏目中关闭此功能！",
      });
    } else if (content.referenceId > 0 && content.sourceId > 0) {
      utils.openLayer({
        title: "编辑引用内容",
        url: utils.getCmsUrl("contentsLayerReference", {
          siteId: this.siteId,
          channelId: content.channelId,
          contentId: content.id,
          page: this.page,
        }),
        full: true,
      });
    } else {
      utils.addTab("编辑内容", this.getEditUrl(content));
    }
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

  getAddUrl: function () {
    return utils.getCmsUrl("editor", {
      siteId: this.siteId,
      channelId: this.channelId,
      page: this.page,
      tabName: utils.getTabName(),
    });
  },

  getEditUrl: function (content) {
    return utils.getCmsUrl("editor", {
      siteId: this.siteId,
      channelId: content.channelId,
      contentId: content.id,
      page: this.page,
      tabName: utils.getTabName(),
    });
  },

  btnCreateClick: function () {
    var $this = this;
    if (!this.isContentChecked) return;

    utils.loading(this, true);
    $api
      .post($url + "/actions/create", {
        siteId: $this.siteId,
        channelContentIds: this.channelContentIdsString,
      })
      .then(function (response) {
        var res = response.data;

        utils.addTab("生成进度查看", utils.getCmsUrl("createStatus", { siteId: $this.siteId }));
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnLayerClick: function (options) {
    if (this.isChangeBanned) {
      if (options.name === "Delete") {
        return utils.alertWarning({
          title: "禁止删除内容",
          text: "栏目已开启禁止维护内容(添加/修改/删除)功能，删除内容请先在栏目中关闭此功能！",
        });
      } else if (options.name === "Word" || options.name === "Import" || options.name === "Add") {
        return utils.alertWarning({
          title: "禁止添加内容",
          text: "栏目已开启禁止维护内容(添加/修改/删除)功能，添加内容请先在栏目中关闭此功能！",
        });
      } else if (
        options.name === "Attributes" ||
        options.name === "Taxis" ||
        options.name === "Check" ||
        options.name === "Group" ||
        options.name === "Tag" ||
        options.name === "Arrange" ||
        options.name === "Hits"
      ) {
        return utils.alertWarning({
          title: "禁止修改内容",
          text: "栏目已开启禁止维护内容(添加/修改/删除)功能，修改内容请先在栏目中关闭此功能！",
        });
      }
    }
    var query = {
      siteId: this.siteId,
      page: this.page,
    };

    if (options.channelId) {
      query.channelId = options.channelId;
    } else {
      query.channelId = this.channelId;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }

    if (options.withContents) {
      if (!this.isContentChecked) return;
      query.channelContentIds = this.channelContentIdsString;
    }

    if (options.saveIds) {
      if (!this.isContentChecked) return;
      this.apiSaveIds(function (fileName) {
        query.fileName = fileName;
        options.url = utils.getCmsUrl("contentsLayer" + options.name, query);
        utils.openLayer(options);
      });
    } else {
      options.url = utils.getCmsUrl("contentsLayer" + options.name, query);
      utils.openLayer(options);
    }
  },

  btnContentStateClick: function (content) {
    utils.openLayer({
      title: "查看审核状态",
      url: utils.getCmsUrl("contentsLayerState", {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id,
      }),
      full: true,
    });
  },

  btnKnowledgeClick: function (content) {
    utils.addTab(
      "内容知识库",
      utils.getRootUrl("dataset/documents/", {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id,
      })
    );
  },

  btnCommandsClick: function (command) {
    if (!command || command.indexOf(",") === -1) return;

    var first = command.split(",")[0];
    var second = command.split(",")[1];
    for (var menu of this.contentsMenus) {
      if (first === menu.id) {
        for (var child of menu.children) {
          if (second === child.id) {
            this.btnCommandClick(child);
            break;
          }
        }
        break;
      }
    }
  },

  btnCommandClick: function (menu) {
    var args = {
      siteId: this.siteId,
      channelId: this.channelId,
      isContentChecked: this.isContentChecked,
      channelContentIds: this.channelContentIdsString,
    };
    if (menu.click) {
      eval(menu.click + "(args)");
      return;
    }

    var url = utils.addQuery(menu.link, args);
    if (menu.target == "_layer") {
      utils.openLayer({
        title: menu.text,
        url: url,
        full: true,
      });
    } else if (menu.target == "_self") {
      location.href = url;
    } else if (menu.target == "_parent") {
      parent.location.href = url;
    } else if (menu.target == "_top") {
      top.location.href = url;
    } else if (menu.target == "_blank") {
      window.open(url);
    } else {
      utils.addTab(menu.text, url);
    }
  },

  btnMenuClick: function (menu, content) {
    if (menu.click) {
      eval(menu.click + "(content)");
      return;
    }
    var url = utils.addQuery(menu.link, {
      siteId: this.siteId,
      channelId: content.channelId,
      contentId: content.id,
    });

    if (menu.target == "_layer") {
      utils.openLayer({
        title: menu.text,
        url: url,
        full: true,
      });
    } else if (menu.target == "_self") {
      location.href = url;
    } else if (menu.target == "_parent") {
      parent.location.href = url;
    } else if (menu.target == "_top") {
      top.location.href = url;
    } else if (menu.target == "_blank") {
      window.open(url);
    } else {
      utils.addTab(menu.text, url);
    }
  },

  scrollToTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  tableRowClassName: function (scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return "current-row";
    }
    return "";
  },

  btnChannelClick: function (data) {
    if (data.disabled) return;
    this.channelId = data.value;
    this.apiList(data.value, 1);
  },

  filterNode: function (value, data) {
    if (!value) return true;
    return data.label.indexOf(value) !== -1 || data.value + "" === value;
  },

  handleHeaderDragend: function (newWidth, oldWidth, header) {
    var prevColumn = null;
    var nextColumn = null;

    for (var i = 0; i < this.$refs.multipleTable.columns.length; i++) {
      var column = this.$refs.multipleTable.columns[i];
      if (!column.columnKey || !column.resizable) continue;
      if (prevColumn) {
        nextColumn = column;
      } else if (column.columnKey == header.columnKey) {
        prevColumn = column;
      }
    }

    var diff = oldWidth - newWidth;
    if (nextColumn) {
      nextColumn.width += diff;
    }

    this.apiWidth(
      prevColumn ? prevColumn.columnKey : "",
      prevColumn ? prevColumn.width : 0,
      nextColumn ? nextColumn.columnKey : "",
      nextColumn ? nextColumn.width : 0
    );
  },

  handleSelectionChange: function (val) {
    this.multipleSelection = val;
  },

  toggleSelection: function (row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  handleCurrentChange: function (val) {
    this.apiList(this.channelId, val);
  },

  handleColumnsChange: function () {
    var cells = this.$refs.multipleTable.$el.getElementsByClassName("cell");
    for (var i = 0; i < cells.length; i++) {
      var cell = cells[i];
      cell.style.removeProperty("width");
    }

    var listColumns = _.filter(this.columns, function (o) {
      return o.isList;
    });
    var attributeNames = _.map(listColumns, function (column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  },

  onSort: function (event) {
    var content = this.pageContents[event.oldIndex];
    var isUp = event.oldIndex > event.newIndex;
    var rows = Math.abs(event.oldIndex - event.newIndex);
    this.apiOrder(content.channelId, content.id, isUp, rows);
  },

  btnCloseClick: function () {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  components: {
    ElTableDraggable,
  },
  data: data,
  methods: methods,
  computed: {
    isAdvanced: function () {
      if (this.checkedLevels.length !== this.advancedForm.checkedLevels.length) return true;
      if (
        this.advancedForm.isTop ||
        this.advancedForm.isRecommend ||
        this.advancedForm.isHot ||
        this.advancedForm.isColor ||
        this.advancedForm.isIndex
      )
        return true;
      if (this.advancedForm.groupNames.length > 0 || this.advancedForm.tagNames.length > 0)
        return true;
      return false;
    },

    isContentChecked: function () {
      return this.multipleSelection.length > 0;
    },

    channelContentIds: function () {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var content = this.multipleSelection[i];
        retVal.push({
          channelId: content.channelId,
          id: content.id,
        });
      }
      return retVal;
    },

    channelContentIdsString: function () {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var content = this.multipleSelection[i];
        retVal.push(content.channelId + "_" + content.id);
      }
      return retVal.join(",");
    },
  },
  watch: {
    filterText: function (val) {
      this.$refs.tree.filter(val);
    },
  },
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    this.apiTree(false);
  },
});
