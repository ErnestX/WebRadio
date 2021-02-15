# WebRadio

The backend of a WAV file streamer

Notable classes: 
  - [MyBufferedWaveProvider.cs](https://github.com/ErnestX/WebRadio/blob/master/Radio/MyBufferedWaveProvider.cs), [MyBufferedWaveProviderTests.cs](https://github.com/ErnestX/WebRadio/blob/master/Radio.UnitTests/MyBufferedWaveProviderTests.cs)
  - [Bufferer.cs](https://github.com/ErnestX/WebRadio/blob/master/Radio/Bufferer.cs)
  - [BufferReuseManager.cs](https://github.com/ErnestX/WebRadio/blob/master/Radio/BufferReuseManager.cs), [BufferReuseManagerTests.cs](https://github.com/ErnestX/WebRadio/blob/master/Radio.UnitTests/BufferReuseManagerTests.cs) (I didn't know about circular buffers but it was good practice) 
