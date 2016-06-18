'use strict';

var express = require('express');
var app = express();
var pg = require('pg');

app.get("/products", function (req, res, next) {
  pg.connect(process.env.POSTGRES_URL, function (err, client, done) {
    if (err) {
      return next(err);
    }
    client.query("SELECT * from product ORDER BY id ASC", function (err, result) {
      done();
      if (err) {
        return next(err);
      }
      res.json(result.rows);
    });
  });
});

app.listen(process.env.PORT, function () {
  console.log('READY');
});