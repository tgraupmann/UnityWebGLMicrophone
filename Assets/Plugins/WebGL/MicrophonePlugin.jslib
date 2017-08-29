var MicrophonePlugin = {

  Init: function() {

    console.log("Init:");
  
    document.dataArray = undefined;
  
    var constraints = {
      audio: {
        optional: [{
          sourceId: "audioSource"
        }]
      }
    };
    navigator.getUserMedia = ( navigator.getUserMedia ||
				   navigator.webkitGetUserMedia ||
				   navigator.mozGetUserMedia ||
				   navigator.msGetUserMedia);
    navigator.getUserMedia(constraints, function(stream) {
      console.log('navigator.getUserMedia successCallback: ', stream);
	  
	  document.position = 0;

      document.audioContext = new AudioContext();
	  document.tempSize = 1024;
	  document.tempArray = new Float32Array(document.tempSize)
      document.analyser = document.audioContext.createAnalyser();
      document.analyser.minDecibels = -90;
      document.analyser.maxDecibels = -10;
      document.analyser.smoothingTimeConstant = 0.85;

      document.mediaRecorder = new MediaRecorder(stream);

      document.source = document.audioContext.createMediaStreamSource(stream);

      document.source.connect(document.analyser);

	  document.mediaRecorder.start();
      console.log(document.mediaRecorder.state);

	  document.readDataOnInterval = function() {

	    if (document.dataArray == undefined) {
		  setTimeout(document.readDataOnInterval, 1000); //wait to be set
		  return;
		}

	    document.tempInterval = Math.floor(document.tempSize / document.dataArray.length * 1000);

	    // read the next chunk after interval
	    setTimeout(document.readDataOnInterval, document.tempInterval); //if mic is still active

		if (document.dataArray == undefined) {
		  return;
		}

		//read the temp data buffer
	    document.analyser.getFloatTimeDomainData(document.tempArray);

		var j = (document.position + document.dataArray.length - document.tempSize) % document.dataArray.length;
		for (var i = 0; i < document.tempSize; ++i) {
		  document.dataArray[j] = document.tempArray[i];
		  j = (j + 1) % document.dataArray.length;
		}
		document.position = (document.position + document.tempSize) % document.dataArray.length;

	  };

	  document.readDataOnInterval();


    }, function(error) {
      console.log('navigator.getUserMedia errorCallback: ', error);
    });
  },
  
  GetMicrophonePosition: function() {
    return document.position;
  },
  
  hasGetUserMedia: function() {
    return !!(navigator.getUserMedia || navigator.webkitGetUserMedia ||
      navigator.mozGetUserMedia || navigator.msGetUserMedia);
  },

  InitJavaScriptSharedArray: function(byteOffset, length) {
    console.log("InitJavaScriptSharedArray:");
	if (buffer == undefined) {
	  console.log("InitJavaScriptSharedArray: buffer is undefined!");
	  return;
	}
	
	console.log("InitJavaScriptSharedArray: length="+length);
	
    document.dataArray = new Float32Array(buffer, byteOffset, length);
  },
  
  QueryAudioInput: function() {

    if (document.mMicrophones == undefined) {
      document.mMicrophones = [];
    }

    // clear
    while (document.mMicrophones.length > 0) {
      document.mMicrophones.pop();
    } 

    if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
      console.log("enumerateDevices() not supported.");
    } else {
      // List microphones
      navigator.mediaDevices.enumerateDevices()
      .then(function(devices) {
        devices.forEach(function(device) {
          console.log("QueryAudioInput: kind="+device.kind + " device=", device, " id=" + device.deviceId);
          if (device.kind === 'audioinput') {
            document.mMicrophones.push(device.deviceId);
          }
        });
      })
      .catch(function(err) {
        console.log(err.name + ": " + err.message);
      });
    }
  },
  
  GetNumberOfMicrophones: function() {
    if (document.mMicrophones == undefined) {
      document.mMicrophones = [];
    }  
    //console.log("GetNumberOfMicrophones length="+document.mMicrophones.length);
    return document.mMicrophones.length;
  },
  
  GetMicrophoneDeviceName: function(index) {
    if (document.mMicrophones == undefined) {
      document.mMicrophones = [];
    }
    console.log("GetMicrophoneDeviceName");
    
    var returnStr = "Invalid Index";
    if (index >= 0 && index < document.mMicrophones.length) {
      returnStr = document.mMicrophones[index];
    }
  
    var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
    writeStringToMemory(returnStr, buffer);
    return buffer;
  }
};

mergeInto(LibraryManager.library, MicrophonePlugin);
