const express = require('express');
const app = express();
app.use(function (req, res, next) {
  res.setHeader('Access-Control-Allow-Origin', '*');
  next();
});

app.use(express.static('.'));

app.use(express.static('..'));

app.listen(5001, function () {
  console.log('Example app listening on port 5001!');
})
