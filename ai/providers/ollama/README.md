# 概述

<p align="center">
  <img src="https://ssrag.com/docs/images/getting-started/models/ollama/01.png" />
</p>

[Ollama](https://ollama.com/) 是一款跨平台（MacOS、Windows、Linux）推理框架客户端，旨在便捷部署如 Llama 2、Mistral、Llava 等大语言模型（LLM）。Ollama 支持一键安装和本地运行 LLM，所有数据均保留在本地设备上，从而大幅提升数据隐私与安全性。

## 快速集成

### 下载并启动 Ollama

1. 下载 Ollama

   访问 [https://ollama.com/download](https://ollama.com/download) ，根据你的操作系统下载 Ollama 客户端。

2. 运行 Ollama 并进行对话

   ```bash
   ollama run deepseek-r1
   ```

   此操作将启动 Ollama，下载并运行 deepseek-r1 模型，启动成功后，Ollama 会在本地 11434 端口启动 API 服务，可通过 `http://localhost:11434` 访问，浏览器将显示文本：`Ollama is running`。

   如需使用其他模型，请访问 [Ollama 模型库](https://ollama.com/search) 获取更多信息。

3. 在 SSRAG 中集成 Ollama

   进入 `项目管理 > 模型设置`，点击 `Ollama` 图标，系统将弹出设置窗口，在窗口中填写以下信息：

   ![在 SSRAG 中集成 Ollama](https://ssrag.com/docs/images/getting-started/models/ollama/02.png)

   - 基础 URL: `http://<your-ollama-endpoint-domain>:11434`

     填写 Ollama 服务可访问的基础 URL。如果填写公网地址后仍然报错，请参考[常见问题](#faq)，并通过修改环境变量使 Ollama 服务可被所有 IP 访问。

     如果 SSRAG 通过 Docker 部署，建议填写本地局域网 IP 地址，例如 `http://192.168.1.100:11434` 或 `http://host.docker.internal:11434` 进行访问。

     如果是本地源码部署，则填写 `http://localhost:11434`。

   点击“保存”后，若无报错即可在应用中使用该模型。

   嵌入（Embedding）模型的集成方式与 LLM 类似，只需将模型类型切换为“文本嵌入”即可。

4. 使用 Ollama 模型

进入需要配置的应用的「应用配置」页面，在 AI 设置 中选择 `Ollama` 对应的模型，保存后即可使用。

![使用 Ollama 模型](https://ssrag.com/docs/images/getting-started/models/ollama/03.png)

## FAQ

### ⚠️ 如果你使用 Docker 部署 SSRAG 和 Ollama，可能会遇到如下报错：

```bash
httpconnectionpool(host=127.0.0.1, port=11434): max retries exceeded with url:/cpi/chat (Caused by NewConnectionError('<urllib3.connection.HTTPConnection object at 0x7f8562812c20>: fail to establish a new connection:[Errno 111] Connection refused'))

httpconnectionpool(host=localhost, port=11434): max retries exceeded with url:/cpi/chat (Caused by NewConnectionError('<urllib3.connection.HTTPConnection object at 0x7f8562812c20>: fail to establish a new connection:[Errno 111] Connection refused'))
```

出现该错误的原因是 Ollama 服务无法被 Docker 容器访问。`localhost` 通常指的是容器自身，而不是主机或其他容器。

你需要将 Ollama 服务暴露到网络上，才能解决此问题。

### 在 Mac 上设置环境变量

如果 Ollama 作为 macOS 应用运行，环境变量应通过 `launchctl` 设置：

1. 对于每个环境变量，使用 `launchctl setenv` 命令进行设置。

   ```bash
   launchctl setenv OLLAMA_HOST "0.0.0.0"
   ```

2. 重启 Ollama 应用程序。
3. 如果上述方法无效，可以尝试以下方式：

   问题实际上出在 Docker 本身。要让容器访问宿主机，需要连接到 `host.docker.internal`。因此，将服务中的 `localhost` 替换为 `host.docker.internal` 即可正常访问。

   ```bash
   http://host.docker.internal:11434
   ```

### 在 Linux 上设置环境变量

如果 Ollama 作为 systemd 服务运行，环境变量应通过 `systemctl` 进行设置：

1. 通过执行 `systemctl edit ollama.service` 编辑 systemd 服务，这将打开一个编辑器。
2. 在 `[Service]` 部分下，为每个环境变量添加一行 `Environment`。

   ```ini
   [Service]
   Environment="OLLAMA_HOST=0.0.0.0"
   ```

3. 保存并退出编辑器。
4. 重新加载 `systemd` 并重启 Ollama：

   ```bash
   systemctl daemon-reload
   systemctl restart ollama
   ```

### 在 Windows 上设置环境变量

在 Windows 系统中，Ollama 会继承你的用户和系统环境变量。

1. 首先，在任务栏中右键点击 Ollama 图标并选择退出，关闭 Ollama 程序。
2. 打开控制面板，进入系统环境变量设置界面。
3. 为你的用户账户编辑或新建环境变量，例如 `OLLAMA_HOST`、`OLLAMA_MODELS` 等。
4. 点击“确定”或“应用”保存设置。
5. 重新打开一个新的终端窗口，运行 `ollama`。

## 如何将 Ollama 暴露到网络上？

Ollama 默认绑定在 127.0.0.1 的 11434 端口。你可以通过设置 `OLLAMA_HOST` 环境变量来更改绑定地址，从而实现网络访问。
