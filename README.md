<p align="center">
  <a href="https://ssrag.com" target="_blank"><img style="max-width: 400px" src="https://ssrag.com/images/logo/logo.png" alt="SSRAG" /></a>
</p>
<p align="center">
    SSRAG 是基于 LLM 的开源知识库平台，涵盖内容管理（CMS）、知识库问答（RAG）、可视化 AI 工作流编排（Workflow）到智能体（Agent）的全面应用。
</p>

## 产品特性

**开箱即用**

可以部署在本地，也可以使用 SSRAG 云服务，支持开源版本私有化部署，开箱即用。

**内容管理**

SSRAG 架构在 SSCMS 之上，除了将各类文档作为知识库，系统还可以将网站内容直接作为知识库，无需额外配置。

**可视化工作流**

通过 AI 可视化工作流编排能力，能够实现从问题输入到模型输出的完整流程定义，便于调试和设计复杂流程。

**主流大模型**

SSRAG 支持近百种大语言模型及向量模型，涵盖了国内外主流模型，如：Qwen、DeepSeek、OpenAI、Gemini、Llama、Mistral 等，便于你轻松访问、自由切换并对比不同模型的性能。

**数据处理能力**

SSRAG 知识库系统支持各种类型的导入文档格式，包括：PDF、PowerPoint、Word、Excel、HTML、Markdown、文本文件(txt、csv、json、xml...)、ZIP 压缩包、EPubs 等。

**渐进式能力升级**

从基础问答（RAG）开始，逐步升级到复杂流程自动化（Workflow）， 最终实现智能体（Agent）的全面应用。

## 快速启动

启动 SSRAG 服务的最简单方法是采用 Docker Compose 运行 docker 目录下的 docker-compose.yml 文件（在运行安装命令之前，请确保您的机器上安装了 Docker 和 Docker Compose）：

```sh
# 克隆 SSRAG 源代码至本地环境
git clone https://github.com/siteserver/ssrag.git
# 进入 SSRAG 源代码的 Docker 目录
cd ssrag/docker
# 复制环境配置文件
cp .env.example .env
# 启动 Docker 容器
docker compose up -d
```

运行后，可以在浏览器上访问 http://localhost/ss-admin/ 进入 SSRAG 后台并开始初始化安装操作。

可以访问 SSRAG 文档中心 [Docker Compose 部署](https://ssrag.com/docs/getting-started/community/docker-compose.html) 查看详细安装步骤。

## 按月迭代

SSRAG 将每月发布新版本，快速响应用户需求，持续优化产品体验。 确保您始终享受到最新的 AI 技术和功能特性。

## 参与贡献

SSRAG 是一款完全开源的产品，源代码托管在 [Github](https://github.com/siteserver/ssrag) 之上，产品 bug 或建议请提交到 [Github issues](https://github.com/siteserver/ssrag/issues/) 中，bug 我们将第一时间修复，建议我们将尽可能通过产品或插件满足；从提交问题，撰写文档，到提交代码，我们欢迎并期待任何形式的贡献！

您也可以通过 [码云](https://gitee.com/siteserver/ssrag) 获取最新源码或 [提交 bug 或建议](https://gitee.com/siteserver/ssrag/issues)。

如果您觉得我们的项目还不错，还请在 Github 或 Gitee 上给我们点个 Star，我们需要您的支持和鼓励~~

## 关注最新动态

[![qrcode](https://sscms.com/assets/images/qrcode_for_wx.jpg)](https://ssrag.com/)

## License

[GNU Affero General Public License v3.0](LICENSE)

Copyright (C) 2003-2025 SSRAG
