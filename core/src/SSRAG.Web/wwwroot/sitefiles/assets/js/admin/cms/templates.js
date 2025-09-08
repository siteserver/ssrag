﻿var $url = '/cms/templates/templates';
var $urlDelete = $url + '/actions/delete';
var $urlDefault = $url + '/actions/default';
var $urlImport = $url + '/actions/import';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  templateType: utils.getQueryString("templateType") || 'All',
  channels: null,
  allTemplates: null,
  templates: null,
  channelIds: [],
  keyword: '',
  urlUpload: null,
});

var methods = {
  apiList: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.channels;
      $this.allTemplates = $this.templates = res.templates;
      $this.reload();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDefault: function (template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDefault, {
      siteId: this.siteId,
      templateId: template.id
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.channels;
      $this.allTemplates = res.templates;
      $this.reload();
      utils.success('默认模板设置成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      templateId: template.id
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.channels;
      $this.allTemplates = res.templates;
      $this.reload();
      utils.success('模板删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  tableRowClassName(scope) {
    if (scope.row.defaultTemplate) {
      return 'default-row';
    }
    return '';
  },

  getTemplateType: function(templateType) {
    if (templateType === 'IndexPageTemplate') {
      return '首页模板';
    } else if (templateType === 'ChannelTemplate') {
      return '栏目模板';
    } else if (templateType === 'ContentTemplate') {
      return '内容模板';
    } else if (templateType === 'FileTemplate') {
      return '单页模板';
    }
    return '';
  },

  btnDefaultClick: function (template) {
    var $this = this;

    utils.alertDelete({
      title: '设置默认模板',
      text: '此操作将把模板 ' + template.templateName + ' 设为默认' + this.getTemplateType(template.templateType) + '，确认吗？',
      button: '确 认',
      callback: function () {
        $this.apiDefault(template);
      }
    });
  },

  btnCopyClick: function(template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/copy', {
      siteId: this.siteId,
      templateId: template.id
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.channels;
      $this.allTemplates = res.templates;
      $this.reload();
      utils.success('快速复制成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnExportClick: function(template) {
    window.open($apiUrl + $url + '/actions/export' + '?siteId=' + this.siteId + '&templateId=' + template.id + '&access_token=' + $token);
  },

  btnCreateClick: function(template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/create', {
      siteId: this.siteId,
      templateId: template.id
    }).then(function (response) {
      var res = response.data;

      utils.addTab('生成进度查看', utils.getCmsUrl('createStatus', {siteId: $this.siteId}));
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDeleteClick: function (template) {
    var $this = this;

    utils.alertDelete({
      title: '删除模板',
      text: '此操作将删除模板 ' + template.templateName + '，确认吗？',
      callback: function () {
        $this.apiDelete(template);
      }
    });
  },

  btnEditClick: function(template) {
    window.open(this.getEditorUrl(template.templateType, template.id));
  },

  getEditorUrl: function(templateType, templateId) {
    return utils.getCmsUrl('templatesEditor', {
      siteId: this.siteId,
      templateId: templateId,
      templateType: templateType,
      accessToken: $token
    });
  },

  btnUserCountClick(template) {
    if (template.templateType == 'ChannelTemplate') {
      utils.addTab('匹配模板', utils.getCmsUrl('templatesMatch', {
        siteId: this.siteId,
        channelTemplateId: template.id
      }));
    } else if (template.templateType == 'ContentTemplate') {
      utils.addTab('匹配模板', utils.getCmsUrl('templatesMatch', {
        siteId: this.siteId,
        contentTemplateId: template.id
      }));
    }
  },

  reload: function() {
    var $this = this;

    this.templates = _.filter(this.allTemplates, function(o) {
      var isTemplateType = true;
      var isChannels = true;
      var isKeyword = true;
      if ($this.templateType != 'All') {
        isTemplateType = o.templateType === $this.templateType;
      }
      if ($this.channelIds.length > 0) {
        isChannels = false;
        for (var i = 0; i < $this.channelIds.length; i++) {
          var channelId = $this.channelIds[i][$this.channelIds[i].length - 1];
          if (o.channelIds && o.channelIds.indexOf(channelId) !== -1) {
            isChannels = true;
          }
        }
      }
      if ($this.keyword) {
        isKeyword = (o.templateName || '').indexOf($this.keyword) !== -1 || (o.relatedFileName || '').indexOf($this.keyword) !== -1 || (o.createdFileFullName || '').indexOf($this.keyword) !== -1;
      }

      return isTemplateType && isChannels && isKeyword;
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },

  getImportUrl: function() {
    return $apiUrl + $urlImport + '?siteId=' + this.siteId + '&templateType=' + this.templateType;
  },

  uploadBefore(file) {
    var re = /(\.html)$/i;
    if(!re.exec(file.name)) {
      utils.error('上传格式错误，请上传html文件!');
      return false;
    }

    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);

    utils.success('模板文件导入成功');
    location.href = utils.getCmsUrl('templates', {
      siteId: this.siteId,
      templateType: this.templateType
    });
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    this.apiList();
  }
});
