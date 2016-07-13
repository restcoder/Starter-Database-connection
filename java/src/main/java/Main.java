
import java.net.URI;
import java.net.URISyntaxException;
import java.sql.*;
import java.util.ArrayList;
import java.net.URI;
import java.net.URISyntaxException;
import static spark.Spark.*;
import static spark.Spark.get;
import com.google.gson.Gson;



class Product {
  public Integer id;
  public String name;
  public Integer quantity;
  public Product(Integer id, String name, Integer quantity) {
    this.id = id;
    this.name = name;
    this.quantity = quantity;
  }
}

public class Main {

  private static Connection getConnection() throws URISyntaxException, SQLException {
      URI dbUri = new URI(System.getenv("POSTGRES_URL"));
      String[] split = dbUri.getUserInfo().split(":");
      String username = split[0];
      String password = "";
      if (split.length > 1) {
        password = split[1];
      }
      String dbUrl = "jdbc:postgresql://" + dbUri.getHost() + ':' + dbUri.getPort() + dbUri.getPath();
  
      return DriverManager.getConnection(dbUrl, username, password);
  }

  public static void main(String[] args) {
    port(Integer.valueOf(System.getenv("PORT")));
    Gson gson = new Gson();

    get("/products", (req, res) -> {
        res.type("application/json");
        Connection connection = getConnection();
        Statement stmt = connection.createStatement();
        ResultSet rs = stmt.executeQuery("SELECT * FROM product");
        ArrayList<Product> output = new ArrayList<Product>();
        while (rs.next()) {
          output.add(new Product(rs.getInt("id"), rs.getString("name"), rs.getInt("quantity")));
        }
        return output;
    }, gson::toJson);

    awaitInitialization();
    System.out.println("READY");
  }

}
