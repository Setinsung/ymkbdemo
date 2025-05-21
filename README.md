
# YmKB 项目启动说明

## 一、环境配置说明

- 首先确保环境安装了`.NET9 SDK`。

### （一）数据库配置（默认使用SQLite）
- **配置文件路径**  
   - `.\YmKB\src\YmKB.API\appsettings.json`

- **配置项说明**  


```json
"DatabaseSettings": {
  "DatabaseType": "SQLite",       // 数据库类型（支持SQLite/MySQL等）
  "ConnectionString": "Data Source=ymkb.db"  // SQLite无需数据库服务，直接指定文件路径
}
```
- **无需额外服务**：使用SQLite时，系统自动创建/连接数据库文件`ymkb.db`
- **扩展其他数据库**：如需切换为MySQL，需修改`DatabaseType`并配置完整连接字符串（需提前安装数据库服务）

### （二）向量数据库配置（Qdrant服务）
- **服务要求**  
   - 需提前运行Qdrant服务（建议版本≥1.4.0）
   - 支持本地部署或远程服务（确保网络可达）
- **配置文件路径** 
   - 同数据库配置文件：`.\YmKB\src\YmKB.API\appsettings.json`
- **配置项说明**  

```json
"QdrantSettings": {
  "Endpoint": "http://localhost:6333",   // Qdrant服务地址（本地默认端口6333）
  "ApiKey": ""                     // 访问密钥（如有认证需求）
}
```
- **本地快速启动**：可通过Docker一键启动Qdrant：  

```bash
docker run -p 6333:6333 qdrant/qdrant
```


## 二、项目启动步骤

### （一）后端服务启动（API）
- **目录路径**  
   - `.\YmKB\src\YmKB.API\`

- **执行命令**  

```bash
dotnet run
```

- **验证方式**
   - 访问 `http://localhost:5045/scalara/v1` 查看API文档


### （二）前端界面启动（UI）
- **目录路径**  
   - `.\YmKB\src\YmKB.UI\`

- **执行命令** 

```bash
dotnet run
```

- **访问地址**  
   - 打开浏览器访问 `http://localhost:5128`