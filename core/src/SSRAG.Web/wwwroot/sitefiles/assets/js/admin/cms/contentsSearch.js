﻿var $url = "/cms/contents/contentsSearch";
var $urlTree = $url + "/actions/tree";
var $urlList = $url + "/actions/list";
var $urlColumns = $url + "/actions/columns";
var $urlSaveAllIds = $url + "/actions/saveAllIds";
var $urlSaveIds = $url + "/actions/saveIds";

var $defaultWidth = 160;

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  root: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: [],

  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  titleColumn: null,
  bodyColumn: null,
  columns: null,
  permissions: null,
  menus: null,

  tableMaxHeight: 999999999999,
  multipleSelection: [],

  checkedColumns: [],
  searchColumns: [],
  isKnowledge: false,

  searchForm: {
    searchType: "All",
    channelIds: [utils.getQueryInt("siteId")],
    isAllContents: true,
    startDate: null,
    endDate: null,
    checkedLevels: [],
    isTop: false,
    isRecommend: false,
    isHot: false,
    isColor: false,
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

  apiTree: function () {
    var $this = this;

    $api
      .post($urlTree, {
        siteId: this.siteId,
      })
      .then(function (response) {
        var res = response.data;

        $this.root = res.root;
        $this.searchForm.channelIds = [res.channelIds];
        $this.siteUrl = res.siteUrl;
        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.checkedLevels = res.checkedLevels;
        $this.titleColumn = res.titleColumn;
        $this.bodyColumn = res.bodyColumn;
        $this.columns = res.columns;
        $this.permissions = res.permissions;
        $this.permissions.isAdd = false;
        var keyword = utils.getQueryString("keyword") || "";
        $this.searchColumns.push({
          attributeName: $this.titleColumn.attributeName,
          displayName: $this.titleColumn.displayName,
          value: keyword,
        });
        $this.searchColumns.push({
          attributeName: $this.bodyColumn.attributeName,
          displayName: $this.bodyColumn.displayName,
          value: "",
        });

        $this.searchForm.checkedLevels = _.map(res.checkedLevels, function (x) {
          return x.label;
        });
        if (keyword) {
          $this.apiList($this.siteId, 1);
        } else {
          utils.loading($this, false);
        }
      })
      .catch(function (error) {
        utils.loading($this, false);
        utils.error(error);
      });
  },

  getSearchQuery: function (page) {
    var channelIds = [];
    for (var i = 0; i < this.searchForm.channelIds.length; i++) {
      var obj = this.searchForm.channelIds[i];
      if (Array.isArray(obj)) {
        channelIds.push(obj[obj.length - 1]);
      } else {
        channelIds.push(obj);
      }
    }

    var items = [];
    for (var i = 0; i < this.searchColumns.length; i++) {
      var column = this.searchColumns[i];
      if (column.attributeName && column.value) {
        items.push({
          key: column.attributeName,
          value: column.value,
        });
      }
    }

    return {
      siteId: this.siteId,
      searchType: this.searchForm.searchType,
      channelIds: channelIds,
      isAllContents: this.searchForm.isAllContents,
      startDate: this.searchForm.startDate,
      endDate: this.searchForm.endDate,
      items: items,
      page: page,
      isAdvanced: this.isAdvanced,
      isCheckedLevels: this.isCheckedLevels,
      checkedLevels: this.searchForm.checkedLevels,
      isTop: this.searchForm.isTop,
      isRecommend: this.searchForm.isRecommend,
      isHot: this.searchForm.isHot,
      isColor: this.searchForm.isColor,
      groupNames: this.searchForm.groupNames,
      tagNames: this.searchForm.tagNames,
    };
  },

  apiList: function (useless, page, message) {
    var $this = this;

    var query = this.getSearchQuery(page);
    utils.loading(this, true);
    $api
      .post($urlList, query)
      .then(function (response) {
        var res = response.data;

        $this.pageContents = res.pageContents;
        $this.total = res.total;
        $this.pageSize = res.pageSize;
        $this.page = page;

        if (message) {
          utils.success(message);
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
        attributeNames: attributeNames,
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  apiSaveAllIds: function (callback) {
    var $this = this;

    var query = this.getSearchQuery(0);
    utils.loading(this, true);
    $api
      .post($urlSaveAllIds, query)
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

  btnSelectColumnClick: function (column) {
    var searchColumn = _.find(this.searchColumns, function (o) {
      return o.attributeName == column.attributeName;
    });
    if (searchColumn) {
      this.searchColumns.splice(this.searchColumns.indexOf(searchColumn), 1);
    } else {
      this.searchColumns.push({
        attributeName: column.attributeName,
        displayName: column.displayName,
        value: "",
      });
    }
  },

  getColumnEffect: function (column) {
    var searchColumn = _.find(this.searchColumns, function (o) {
      return o.attributeName == column.attributeName;
    });
    return searchColumn ? "dark" : "plain";
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

  btnTabClick: function () {
    if (this.pageContents) {
      this.btnSearchClick();
    }
  },

  btnSearchClick: function () {
    var $this = this;

    this.$refs.searchForm.validate(function (valid) {
      if (valid) {
        $this.apiList($this.siteId, 1);
      }
    });
  },

  btnEditClick: function (content) {
    utils.addTab("编辑内容", this.getEditUrl(content));
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
    utils.addTab("添加内容", this.getAddUrl());
  },

  btnImportClick: function (command) {
    if (command === "Word") {
      this.btnLayerClick({ title: "批量导入Word", name: "Word", full: true });
    } else if (command === "Import") {
      this.btnLayerClick({ title: "批量导入", name: "Import", full: true });
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
    } else if (command === "Copy") {
      var $this = this;
      this.apiSaveAllIds(function (fileName) {
        $this.btnLayerClick({ title: "批量复制", name: "Copy", full: true, fileName: fileName });
      });
    } else if (command === "ExportAll") {
      var $this = this;
      this.apiSaveAllIds(function (fileName) {
        $this.btnLayerClick({ title: "导出全部", name: "Export", full: true, fileName: fileName });
      });
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
    var query = {
      siteId: this.siteId,
      page: this.page,
    };

    if (options.channelId) {
      query.channelId = options.channelId;
    } else {
      query.channelId = this.siteId;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }

    if (options.withContents) {
      if (!this.isContentChecked) return;
      query.channelContentIds = this.channelContentIdsString;
    }
    if (options.fileName) {
      query.fileName = options.fileName;
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

  scrollToTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  tableRowClassName: function (scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return "current-row";
    }
    return "";
  },

  handleSelectionChange: function (val) {
    this.multipleSelection = val;
  },

  toggleSelection: function (row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  handleCurrentChange: function (val) {
    this.apiList(this.siteId, val);
  },

  handleHeaderDragend: function (newWidth, oldWidth, column) {},

  handleColumnsChange: function () {
    var listColumns = _.filter(this.columns, function (o) {
      return o.isList;
    });
    var attributeNames = _.map(listColumns, function (column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  },

  btnCloseClick: function () {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    isAdvanced: function () {
      if (this.searchForm.searchType !== "All") return true;
      if (this.searchForm.channelIds.length > 1) return true;
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      if (
        this.searchForm.isTop ||
        this.searchForm.isRecommend ||
        this.searchForm.isHot ||
        this.searchForm.isColor
      )
        return true;
      if (this.searchForm.groupNames.length > 0 || this.searchForm.tagNames.length > 0) return true;
      if (this.searchForm.startDate || this.searchForm.endDate) return true;
      for (var i = 0; i < this.searchColumns.length; i++) {
        if (this.searchColumns[i].value) return true;
      }
      return false;
    },

    isCheckedLevels: function () {
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      return false;
    },

    isFiltered: function () {
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      if (
        this.searchForm.isTop ||
        this.searchForm.isRecommend ||
        this.searchForm.isHot ||
        this.searchForm.isColor
      )
        return true;
      if (this.searchForm.groupNames.length > 0 || this.searchForm.tagNames.length > 0) return true;
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
  created: function () {
    utils.keyPress(this.btnSearchClick, this.btnCloseClick);
    this.apiTree();
  },
});
