const fs = require('fs-extra');
const del = require('del');
const gulp = require('gulp');
const through2 = require('through2');
const minifier = require('gulp-minifier');
const minify = require('gulp-minify');
const rename = require('gulp-rename');
const replace = require('gulp-string-replace');
const filter = require('gulp-filter');
const runSequence = require('gulp4-run-sequence');
const ALY = require('aliyun-sdk');

const version = '0.9.19';
const timestamp = (new Date()).getTime();
let publishDir = '';
let htmlDict = {};
fs.readdirSync('./core/src/SSRAG.Web/Pages/shared/').forEach(fileName => {
  let html = fs.readFileSync('./core/src/SSRAG.Web/Pages/shared/' + fileName, {
    encoding: "utf8",
  });
  htmlDict[fileName] = html;
  htmlDict[fileName.replace('.cshtml', '')] = html;
});

function transform(file, html) {
  let content = new String(file.contents);
  let result = html || '';

  let matches = [...content.matchAll(/@await Html.PartialAsync\("([\s\S]+?)"\)/gi)];
  if (matches) {
    for (let i = 0; i < matches.length; i++) {
      var match = matches[i];
      content = content.replace(match[0], htmlDict[match[1]]);
    }
  }

  let styles = '';
  matches = [...content.matchAll(/<style>([\s\S]+?)<\/style>/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    styles = matches[0][0];
  }
  matches = [...content.matchAll(/@section Styles{([\s\S]+?)}/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    styles = matches[0][1] + styles;
  }
  styles = styles.replace('@@media ', '@media ');

  let scripts = '';
  matches = [...content.matchAll(/@section Scripts{([\s\S]+?)}/gi)];
  if (matches && matches[0]){
    content = content.replace(matches[0][0], '');
    scripts = matches[0][1];
  }

  result = result.replace('@RenderSection("Styles", required: false)', styles);
  result = result.replace('@RenderBody()', content);
  result = result.replace('@RenderSection("Scripts", required: false)', scripts);
  result = result.replace('@page', '');
  result = result.replace('@{ Layout = "_Layout"; }', '');
  result = result.replace('@{ Layout = "_LayoutHome"; }', '');
  result = result.replace(/\.css"/g, ".css?v=" + timestamp + '"');
  result = result.replace(/\.js"/g, ".js?v=" + timestamp + '"');

  file.contents = Buffer.from(result, 'utf8');
  return file;
}

function writeOss(bucket, key, fileName) {
  var ossStream = require('aliyun-oss-upload-stream')(new ALY.OSS({
    accessKeyId: process.env.OSS_ACCESS_KEY_ID,
    secretAccessKey: process.env.OSS_SECRET_ACCESS_KEY,
    endpoint: 'http://oss-cn-beijing.aliyuncs.com',
    apiVersion: '2013-10-15'
  }));

  var upload = ossStream.upload({
    Bucket: bucket,
    Key: key
  });

  // upload.minPartSize(1048576);

  var read = fs.createReadStream(`./publish/dist/${fileName}`);
  read.pipe(upload);
}

gulp.task('clean', function(){
  return del([`./build/`, `./publish/`], {force:true});
});

// build tasks

gulp.task("build-src", function () {
  return gulp.src("./core/src/**/*").pipe(gulp.dest(`./build/src`));
});

gulp.task("build-sln", function () {
  return gulp.src("./core/build.sln").pipe(gulp.dest(`./build`));
});


gulp.task("build-ss-admin", function () {
  return gulp
    .src("./core/src/SSRAG.Web/Pages/ss-admin/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, htmlDict['_Layout']))
    }))
    .pipe(rename(function (path) {
      if (path.basename != 'index'){
        path.dirname += "/" + path.basename;
        path.basename = "index";
      }
      path.extname = ".html";
    }))
    .pipe(
      minifier({
        minify: true,
        minifyHTML: {
          collapseWhitespace: true,
          conservativeCollapse: true,
        },
      })
    )
    .pipe(gulp.dest(`./build/src/SSRAG.Web/wwwroot/ss-admin`));
});

gulp.task("build-home", function () {
  return gulp
    .src("./core/src/SSRAG.Web/Pages/home/**/*.cshtml")
    .pipe(through2.obj((file, enc, cb) => {
      cb(null, transform(file, htmlDict['_LayoutHome']))
    }))
    .pipe(rename(function (path) {
      if (path.basename != 'index'){
        path.dirname += "/" + path.basename;
        path.basename = "index";
      }
      path.extname = ".html";
    }))
    .pipe(
      minifier({
        minify: true,
        minifyHTML: {
          collapseWhitespace: true,
          conservativeCollapse: true,
        },
      })
    )
    .pipe(gulp.dest(`./build/src/SSRAG.Web/wwwroot/home`));
});

gulp.task('build-clean', function(){
  return del([`./build/src/SSRAG.Web/Pages/ss-admin/**`, `./build/src/SSRAG.Web/Pages/home/**`], {force:true});
});

gulp.task("build", async function () {
  return runSequence(
      "clean",
      "build-src",
      "build-sln",
      "build-ss-admin",
      "build-home",
      "build-clean"
  );
});

// copy tasks

gulp.task("copy-files", async function () {
  fs.copySync('./core/appsettings.json', publishDir + '/appsettings.json');
  fs.copySync('./core/web.config', publishDir + '/web.config');
  fs.copySync('./core/404.html', publishDir + '/wwwroot/404.html');
  fs.copySync('./core/favicon.ico', publishDir + '/wwwroot/favicon.ico');
  fs.copySync('./core/index.html', publishDir + '/wwwroot/index.html');
  fs.removeSync(publishDir + '/appsettings.Development.json');
});

gulp.task("copy-ssrag", async function () {
  fs.removeSync(publishDir + '/web.config');
});

gulp.task("copy-css", function () {
  return gulp
    .src(["./core/src/SSRAG.Web/wwwroot/sitefiles/**/*.css"])
    .pipe(
      minifier({
        minify: true,
        collapseWhitespace: true,
        conservativeCollapse: true,
        minifyJS: false,
        minifyCSS: true,
        minifyHTML: false,
        ignoreFiles: ['.min.css']
      })
    )
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles"));
});

gulp.task("copy-js", function () {
  // 排除 chat-iframe.js 文件
  return gulp
    .src([
      "./core/src/SSRAG.Web/wwwroot/sitefiles/**/*.js",
      "!./core/src/SSRAG.Web/wwwroot/sitefiles/**/chat-iframe.js"
    ])
    .pipe(minify())
    .pipe(filter(['**/*-min.js']))
    .pipe(rename(function (path) {
      path.basename = path.basename.substring(0, path.basename.length - 4);
    }))
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles"));
});

gulp.task("copy-web", async function () {
  fs.copySync('./web/dist/open/', publishDir + "/wwwroot/open", {overwrite: true});
  fs.copySync('./web/dist/assets/', publishDir + "/wwwroot/assets", {overwrite: true});
  fs.copySync('./web/dist/ss-admin/', publishDir + "/wwwroot/ss-admin", { overwrite: true });
  return true;
});

gulp.task("replace-localhost", function () {
  return gulp
    .src("./core/src/SSRAG.Web/wwwroot/sitefiles/assets/js/cloud.js")
    .pipe(replace('http://localhost:6060/', 'https://api.ssrag.com/'))
    .pipe(gulp.dest(publishDir + "/wwwroot/sitefiles/assets/js"));
});

gulp.task("copy", async function (callback) {
  publishDir = `./publish/ssrag-${version}`;

  return runSequence(
    "copy-files",
    "copy-ssrag",
    "copy-css",
    "copy-js",
    "copy-web",
    "replace-localhost",
  );
});

gulp.task("publish-zip", async function () {
  writeOss(process.env.OSS_BUCKET_DL, `ssrag/${version}/ssrag-core-${version}.tar.gz`, `ssrag-core-${version}.tar.gz`);
  writeOss(process.env.OSS_BUCKET_DL, `ssrag/${version}/ssrag-ai-${version}.tar.gz`, `ssrag-ai-${version}.tar.gz`);
  writeOss(process.env.OSS_BUCKET_DL, `ssrag/${version}/ssrag-core-${version}.zip`, `ssrag-core-${version}.zip`);
});
