package com.httpclient;

import java.net.http.HttpClient;

public class HttpClientCustom {
    
    private static HttpClientCustom instance;
    private HttpClient httpClient;

    private HttpClientCustom(){

        this.httpClient = HttpClient.newBuilder()
            .build();

    }

    public static synchronized HttpClientCustom getInstance(){

        if(instance==null){
            instance = new HttpClientCustom();
        }

        return instance;

    }

    public HttpClient getHttpClient(){
        return this.httpClient;
    }

}
