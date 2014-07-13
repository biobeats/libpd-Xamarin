.PHONY: ndkbuild clean

ndkbuild:
	ndk-build TARGET_PLATFORM=android-9

clean:
	rm -rf obj libs/*
