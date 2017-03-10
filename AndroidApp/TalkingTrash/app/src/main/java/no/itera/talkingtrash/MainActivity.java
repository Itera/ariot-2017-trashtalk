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

import java.net.URI;
import java.net.URISyntaxException;

public class MainActivity extends AppCompatActivity {

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
            uri = new URI("ws://trashtalkapi.azurewebsites.net/api/trashstream/" + mDeviceId);
        } catch (URISyntaxException e) {
            e.printStackTrace();
            return;
        }

        mWebSocketClient = new WebSocketClient(uri, new Draft_17()) {
            @Override
            public void onOpen(ServerHandshake serverHandshake) {
                Log.i("Websocket", "Opened");
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
                        TextView textViewStatusPickup = (TextView) findViewById(R.id.status_pickup_text);
                        TextView textViewStatusFillGrade = (TextView) findViewById(R.id.status_fill_grade_text);
                        TextView textViewStatusLid = (TextView) findViewById(R.id.status_lid_text);
                        TextView textViewStatusLastUpdated = (TextView) findViewById(R.id.status_last_updated);
                        ImageView imageViewStatusPickup = (ImageView) findViewById(R.id.status_pickup_image);
                        ProgressBar progressBarStatusFillGrade = (ProgressBar) findViewById(R.id.status_fill_grade_progress_bar);

                        try {
                            textViewStatusPickup.setText(reader.getString("Timestamp"));
                            textViewStatusFillGrade.setText("Fill grade is " + Double.toString(reader.getDouble("FillGrade")));
                            textViewStatusLid.setText("The lid is " + (reader.getBoolean("LidIsClosed") ? "closed" : "open") + ".");
                            textViewStatusLastUpdated.setText("Last updated " + reader.getString("Timestamp"));
                            imageViewStatusPickup.setImageResource(reader.getBoolean("LidIsClosed") ? R.drawable.circle_green : R.drawable.circle_red);
                            progressBarStatusFillGrade.setProgress((int) (reader.getDouble("FillGrade") * 100));
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
