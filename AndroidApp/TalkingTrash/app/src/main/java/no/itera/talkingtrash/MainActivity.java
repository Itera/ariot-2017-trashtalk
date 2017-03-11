package no.itera.talkingtrash;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

import org.java_websocket.client.WebSocketClient;
import org.java_websocket.drafts.Draft_17;
import org.java_websocket.handshake.ServerHandshake;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

public class MainActivity extends AppCompatActivity {

    final static String API_PREFIX = "://trashtalkapi.azurewebsites.net/api/trashstream/";

    WebSocketClient mWebSocketClient = null;
    String mDeviceId = "";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    protected void onResume() {
        super.onResume();
        SharedPreferences SP = PreferenceManager.getDefaultSharedPreferences(getBaseContext());
        mDeviceId = SP.getString("device_id", "");
        connectWebSocket();
    }

    @Override
    protected void onPause() {
        super.onPause();
        mWebSocketClient.close();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle item selection
        switch (item.getItemId()) {
            case R.id.action_settings:
                Intent i = new Intent(this, SettingsActivity.class);
                startActivity(i);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private void connectWebSocket() {
        URI uri;
        try {
            uri = new URI("ws" + API_PREFIX + mDeviceId);
        } catch (URISyntaxException e) {
            e.printStackTrace();
            return;
        }

        mWebSocketClient = new WebSocketClient(uri, new Draft_17()) {
            @Override
            public void onOpen(ServerHandshake serverHandshake) {
                Log.i("Websocket", "Opened");

                HttpURLConnection urlConnection = null;
                try {
                    URL url = new URL("http" + API_PREFIX + "fetch/" + mDeviceId);
                    urlConnection = (HttpURLConnection) url.openConnection();
                    urlConnection.getResponseCode();
                } catch (IOException e) {
                    e.printStackTrace();
                } finally {
                    if (urlConnection != null) {
                        urlConnection.disconnect();
                    }
                }
            }

            @Override
            public void onMessage(String message) {
                final JSONObject reader;
                try {
                    reader = new JSONObject(message);
                } catch (JSONException e) {
                    e.printStackTrace();
                    return;
                }

                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        TextView textViewStatusLid = (TextView) findViewById(R.id.status_lid_text);
                        TextView textViewStatusFillGrade = (TextView) findViewById(R.id.status_fill_grade_text);
                        TextView textViewStatusLastUpdated = (TextView) findViewById(R.id.status_last_updated);
                        ImageView imageViewStatusLid = (ImageView) findViewById(R.id.status_lid_image);
                        ProgressBar progressBarStatusFillGrade = (ProgressBar) findViewById(R.id.status_fill_grade_progress_bar);

                        try {
                            int percentFull = (int) (reader.getDouble("FillGrade") * 100);
                            SimpleDateFormat parser = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
                            String lastUpdated = "";
                            try {
                                Date date = parser.parse(reader.getString("Timestamp"));
                                SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                                lastUpdated = formatter.format(date);
                            } catch (ParseException e) {
                                e.printStackTrace();
                            }

                            textViewStatusLid.setText("The lid is " + (reader.getBoolean("LidIsClosed") ? "closed" : "open"));
                            textViewStatusFillGrade.setText("The can is " + Integer.toString(percentFull) + "% full");
                            textViewStatusLastUpdated.setText("Last updated at " + lastUpdated);
                            imageViewStatusLid.setImageResource(reader.getBoolean("LidIsClosed") ? R.drawable.circle_green : R.drawable.circle_red);
                            progressBarStatusFillGrade.setProgress(percentFull);
                        } catch (JSONException e) {
                            e.printStackTrace();
                            return;
                        }
                    }
                });
            }

            @Override
            public void onClose(int i, String s, boolean b) {
                Log.i("Websocket", "Closed " + s);
            }

            @Override
            public void onError(Exception e) {
                Log.i("Websocket", "Error " + e.getMessage());
            }
        };
        mWebSocketClient.connect();
    }
}
