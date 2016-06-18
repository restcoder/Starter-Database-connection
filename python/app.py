from flask import Flask, Response
from flask.ext.sqlalchemy import SQLAlchemy
from flask import jsonify
from threading import Thread
from config import Config
import time
import sys
import json
import os
import models

app = Flask(__name__)
app.config.from_object(Config)
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

@app.route("/products")
def products():
    try:
      products = models.Product.query.order_by(models.Product.id.asc()).all()
      products = [
          {
              "id": product.id,
              "name": product.name,
              "quantity": product.quantity
          }
          for product in products]
      return Response(json.dumps(products), mimetype='application/json')
    except Exception as e:
        return e.__doc__ + e.message, 500

class readyThread(Thread):
    def run(self):
        time.sleep(1)
        sys.stdout.write("READY")
        sys.stdout.flush()

if __name__ == "__main__":
    readyThread().start()
    app.run(host='0.0.0.0', port=int(os.getenv('PORT')))